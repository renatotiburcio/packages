#!/usr/bin/env bun
// Bundles all TS and SCSS assets for XForge.DesignSystem.
// Replaces manual dotnet-bundler / npm scripts with a single Bun-driven pipeline.
import { Glob } from "bun";
import * as esbuild from "esbuild";
import * as sass from "sass";

const outdir = "wwwroot/dist";
console.log(`[build] Output dir: ${outdir}`);

// 1. Bundle TypeScript modules
const tsEntryPoints = await Array.fromAsync(new Glob("src/**/*.ts").scan({ cwd: "." }));
console.log(`[build] TS entries: ${tsEntryPoints.length}`);
await esbuild.build({
  entryPoints: tsEntryPoints,
  bundle: true,
  outdir,
  format: "esm",
  target: "es2022",
  minify: true,
  sourcemap: "external",
});

// 2. Compile SCSS
const scssEntryPoints = await Array.fromAsync(new Glob("src/**/*.scss").scan({ cwd: "." }));
console.log(`[build] SCSS entries: ${scssEntryPoints.length}`);
for (const file of scssEntryPoints) {
  const result = sass.compile(file, { style: "compressed" });
  const out = `${outdir}/${file.replace(/\.scss$/, ".css")}`;
  await Bun.write(out, result.css);
}

console.log("[build] Done");