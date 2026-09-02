// Minimal Corona shell behaviour for Blazor Server.
// The stock template ships several jQuery plugins (misc.js, off-canvas.js, ...)
// that assume a static DOM and elements this app doesn't render (pro-banner,
// perfect-scrollbar, ...). We only need the sidebar toggles, and we bind them
// with delegated handlers so they keep working across Blazor re-renders.
(function () {
    'use strict';

    document.addEventListener('click', function (e) {
        var minimize = e.target.closest('[data-toggle="minimize"]');
        if (minimize) {
            var body = document.body;
            if (body.classList.contains('sidebar-toggle-display') || body.classList.contains('sidebar-absolute')) {
                body.classList.toggle('sidebar-hidden');
            } else {
                body.classList.toggle('sidebar-icon-only');
            }
            return;
        }

        var offcanvas = e.target.closest('[data-toggle="offcanvas"]');
        if (offcanvas) {
            var sidebar = document.querySelector('.sidebar-offcanvas');
            if (sidebar) sidebar.classList.toggle('active');
        }
    });
})();
