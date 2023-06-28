window.blazorHelpers = {
    startUp: () => {
        /**
         * Create Sidebar Wrapper
         */
        let sidebarEl = document.getElementById("sidebar")


        // Fix mobile Menu for blazor PWA behaviour
        var elements = document.getElementsByClassName("blazor-link");
        for (var i = 0; i < elements.length; i++) {
            elements[i].addEventListener('click', function () {
                var w = window.innerWidth;
                if (w <= 1024) {
                    sidebarEl.classList.remove("active");
                }
            });
        }

        // Disable tap zoom on mobile devices
        var lastTouchEnd = 0;
        document.addEventListener('touchend', function (event) {
            var now = (new Date()).getTime();
            if (now - lastTouchEnd <= 300) {
                event.preventDefault();
            }
            lastTouchEnd = now;
        }, false);
    },
    showSweetAlert: (icon, title, html, timer) => {
        Swal.fire({
            icon: icon,
            title: title,
            html: html,
            timer: timer
        });
    },
    showToast: (type, message, timeout = 5000) => {
        toastr.options = {
            "closeButton": false,
            "debug": false,
            "newestOnTop": true,
            "progressBar": true,
            "positionClass": "toast-bottom-right",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": timeout,
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };


        switch (type) {
            case "warning":
                toastr.warning(message);
                break;
            case "error":
                toastr.error(message);
                break;
            default:
                toastr.success(message);
                break;
        }
    },
    setFocusById: (elementId) => {
        var element = document.getElementById(elementId);
        if (element) {
            element.focus();
        }
    },
    open: (url) => {
        window.open(url, '_blank');
    },
    isDesktop: () => {
        var w = window.innerWidth;
        return w > 1024;
    },
    scrollToFragment: (elementId, behavior) => {
        var element = document.getElementById(elementId);

        if (element) {
            element.scrollIntoView({
                behavior: behavior
            });
        }
    }
}





