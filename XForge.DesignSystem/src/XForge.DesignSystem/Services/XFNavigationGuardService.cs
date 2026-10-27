namespace XForge.DesignSystem.Services
{
    public sealed class XFNavigationGuardService
    {
        private readonly Dictionary<Guid, XFNavigationGuardRegistration> registrations = [];

        public bool HasPendingChanges => registrations.Values.Any(registration => registration.HasPendingChanges());

        public IDisposable Register(Func<bool> hasPendingChanges, Func<string?, Task<bool>> confirmNavigationAsync)
        {
            var id = Guid.NewGuid();
            var registration = new XFNavigationGuardRegistration(
                id,
                hasPendingChanges,
                confirmNavigationAsync,
                () => Unregister(id));

            registrations[id] = registration;
            return registration;
        }

        public async Task<bool> ConfirmNavigationAsync(string? targetUri)
        {
            foreach (var registration in registrations.Values.ToArray().Reverse())
            {
                if (!registration.HasPendingChanges())
                {
                    continue;
                }

                var approved = await registration.ConfirmNavigationAsync(targetUri);
                if (!approved)
                {
                    return false;
                }
            }

            return true;
        }

        private void Unregister(Guid id)
        {
            registrations.Remove(id);
        }

        private sealed class XFNavigationGuardRegistration : IDisposable
        {
            private readonly Action dispose;
            private bool disposed;

            public XFNavigationGuardRegistration(
                Guid id,
                Func<bool> hasPendingChanges,
                Func<string?, Task<bool>> confirmNavigationAsync,
                Action dispose)
            {
                Id = id;
                HasPendingChanges = hasPendingChanges;
                ConfirmNavigationAsync = confirmNavigationAsync;
                this.dispose = dispose;
            }

            public Guid Id { get; }
            public Func<bool> HasPendingChanges { get; }
            public Func<string?, Task<bool>> ConfirmNavigationAsync { get; }

            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }

                disposed = true;
                dispose();
            }
        }
    }
}
