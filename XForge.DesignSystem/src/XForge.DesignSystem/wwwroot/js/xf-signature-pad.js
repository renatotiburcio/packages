(function () {
    var pads = new Map();

    function fillBackground(ctx, canvas, color) {
        ctx.save();
        ctx.globalCompositeOperation = 'destination-over';
        ctx.fillStyle = color || '#ffffff';
        ctx.fillRect(0, 0, canvas._xfLogicalWidth || canvas.width, canvas._xfLogicalHeight || canvas.height);
        ctx.restore();
    }

    function snapshot(state) {
        state.history.push({
            value: state.canvas.toDataURL('image/png'),
            strokes: state.strokes.map(function (stroke) {
                return stroke.map(function (point) {
                    return { x: point.x, y: point.y };
                });
            })
        });
        if (state.history.length > 25) {
            state.history.shift();
        }
    }

    function loadImage(state, value) {
        var ctx = state.ctx;
        ctx.clearRect(0, 0, state.logicalWidth, state.logicalHeight);
        if (!value) {
            fillBackground(ctx, state.canvas, state.backgroundColor);
            return;
        }

        var image = new Image();
        image.onload = function () {
            ctx.clearRect(0, 0, state.logicalWidth, state.logicalHeight);
            ctx.drawImage(image, 0, 0, state.logicalWidth, state.logicalHeight);
        };
        image.src = value;
    }

    function distance(a, b) {
        var dx = a.x - b.x;
        var dy = a.y - b.y;
        return Math.sqrt(dx * dx + dy * dy);
    }

    function midpoint(a, b) {
        return {
            x: (a.x + b.x) / 2,
            y: (a.y + b.y) / 2
        };
    }

    function drawStroke(ctx, points) {
        if (!points.length) {
            return;
        }

        ctx.beginPath();

        if (points.length === 1) {
            ctx.arc(points[0].x, points[0].y, ctx.lineWidth / 2, 0, Math.PI * 2);
            ctx.fill();
            return;
        }

        if (points.length === 2) {
            ctx.moveTo(points[0].x, points[0].y);
            ctx.lineTo(points[1].x, points[1].y);
            ctx.stroke();
            return;
        }

        ctx.moveTo(points[0].x, points[0].y);

        for (var index = 1; index < points.length - 2; index++) {
            var center = midpoint(points[index], points[index + 1]);
            ctx.quadraticCurveTo(points[index].x, points[index].y, center.x, center.y);
        }

        var penultimate = points[points.length - 2];
        var last = points[points.length - 1];
        ctx.quadraticCurveTo(penultimate.x, penultimate.y, last.x, last.y);
        ctx.stroke();
    }

    function redraw(state) {
        var ctx = state.ctx;
        ctx.clearRect(0, 0, state.logicalWidth, state.logicalHeight);
        fillBackground(ctx, state.canvas, state.backgroundColor);

        ctx.save();
        ctx.lineCap = 'round';
        ctx.lineJoin = 'round';
        ctx.strokeStyle = state.penColor;
        ctx.fillStyle = state.penColor;
        ctx.lineWidth = state.penWidth;
        ctx.imageSmoothingEnabled = true;
        ctx.imageSmoothingQuality = 'high';

        state.strokes.forEach(function (stroke) {
            drawStroke(ctx, stroke);
        });

        if (state.currentStroke.length) {
            drawStroke(ctx, state.currentStroke);
        }

        ctx.restore();
    }

    function point(canvas, event) {
        var rect = canvas.getBoundingClientRect();
        var scaleX = (canvas._xfLogicalWidth || canvas.width) / rect.width;
        var scaleY = (canvas._xfLogicalHeight || canvas.height) / rect.height;
        return {
            x: (event.clientX - rect.left) * scaleX,
            y: (event.clientY - rect.top) * scaleY
        };
    }

    function release(id) {
        var state = pads.get(id);
        if (!state) {
            return;
        }

        state.canvas.removeEventListener('pointerdown', state.down);
        state.canvas.removeEventListener('pointermove', state.move);
        state.canvas.removeEventListener('pointerup', state.up);
        state.canvas.removeEventListener('pointercancel', state.up);
        pads.delete(id);
    }

    function configureCanvas(canvas) {
        var logicalWidth = Number(canvas.getAttribute('width') || canvas.width || 720);
        var logicalHeight = Number(canvas.getAttribute('height') || canvas.height || 220);
        var ratio = Math.max(window.devicePixelRatio || 1, 1);

        canvas._xfLogicalWidth = logicalWidth;
        canvas._xfLogicalHeight = logicalHeight;
        canvas.style.aspectRatio = logicalWidth + ' / ' + logicalHeight;
        canvas.width = Math.round(logicalWidth * ratio);
        canvas.height = Math.round(logicalHeight * ratio);

        var ctx = canvas.getContext('2d');
        ctx.setTransform(ratio, 0, 0, ratio, 0, 0);
        return { ctx: ctx, logicalWidth: logicalWidth, logicalHeight: logicalHeight };
    }

    window.xfSignaturePad = {
        register: function (id, dotNetReference, options) {
            release(id);

            var canvas = document.getElementById(id);
            if (!canvas) {
                return;
            }

            var configured = configureCanvas(canvas);
            var ctx = configured.ctx;
            var state = {
                canvas: canvas,
                ctx: ctx,
                dotNetReference: dotNetReference,
                drawing: false,
                history: [],
                strokes: [],
                currentStroke: [],
                logicalWidth: configured.logicalWidth,
                logicalHeight: configured.logicalHeight,
                backgroundColor: options.backgroundColor || '#ffffff',
                penColor: options.penColor || '#111827',
                penWidth: Number(options.penWidth || 3.2),
                smoothingDistance: Number(options.smoothingDistance || 1.8)
            };

            ctx.lineCap = 'round';
            ctx.lineJoin = 'round';
            ctx.strokeStyle = state.penColor;
            ctx.fillStyle = state.penColor;
            ctx.lineWidth = state.penWidth;
            ctx.imageSmoothingEnabled = true;
            ctx.imageSmoothingQuality = 'high';
            loadImage(state, options.value);

            state.down = function (event) {
                if (options.readOnly) {
                    return;
                }

                event.preventDefault();
                snapshot(state);
                state.drawing = true;
                var start = point(canvas, event);
                state.currentStroke = [start];
                redraw(state);
                canvas.setPointerCapture(event.pointerId);
            };

            state.move = function (event) {
                if (!state.drawing) {
                    return;
                }

                event.preventDefault();
                var next = point(canvas, event);
                var previous = state.currentStroke[state.currentStroke.length - 1];
                if (previous && distance(previous, next) < state.smoothingDistance) {
                    return;
                }

                state.currentStroke.push(next);
                redraw(state);
            };

            state.up = function (event) {
                if (!state.drawing) {
                    return;
                }

                state.drawing = false;
                if (state.currentStroke.length) {
                    state.strokes.push(state.currentStroke);
                }
                state.currentStroke = [];
                redraw(state);
                try {
                    canvas.releasePointerCapture(event.pointerId);
                } catch (_error) {
                }
                dotNetReference.invokeMethodAsync('SetValueFromJsAsync', canvas.toDataURL('image/png'));
            };

            canvas.addEventListener('pointerdown', state.down);
            canvas.addEventListener('pointermove', state.move);
            canvas.addEventListener('pointerup', state.up);
            canvas.addEventListener('pointercancel', state.up);
            pads.set(id, state);
        },

        clear: function (id) {
            var state = pads.get(id);
            if (!state) {
                return;
            }

            state.ctx.clearRect(0, 0, state.canvas.width, state.canvas.height);
            state.ctx.clearRect(0, 0, state.logicalWidth, state.logicalHeight);
            fillBackground(state.ctx, state.canvas, state.backgroundColor);
            state.strokes = [];
            state.currentStroke = [];
            state.history = [];
        },

        undo: function (id) {
            var state = pads.get(id);
            if (!state) {
                return null;
            }

            var previous = state.history.pop();
            state.strokes = previous
                ? previous.strokes.map(function (stroke) {
                    return stroke.map(function (point) {
                        return { x: point.x, y: point.y };
                    });
                })
                : [];
            state.currentStroke = [];
            redraw(state);
            return previous ? previous.value : null;
        },

        release: release
    };
})();
