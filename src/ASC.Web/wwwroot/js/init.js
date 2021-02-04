(function ($) {
    $(function () {

        $('.button-collapse').sideNav();
        $('.parallax').parallax();

        // Impedir os bot�es voltar e avan�ar do navegador.
        if (window.history && window.history.pushState) {
            window.history.pushState('forward', '', window.location.href);
            $(window).on('popstate', function (e) {
                window.history.pushState('forward', '', window.location.href);
                e.preventDefault();
            });
        }


        //Prevent right-click on entire window
        $(document).ready(function () {
            $(window).on("contextmenu", function () {
                return false;
            });
        });

    }); // end of document ready


    // Future Date Vallidation method.
    $.validator.addMethod('futuredate', function (value, element, params) {
        var selectedDate = new Date(value),
            now = new Date(),
            futureDate = new Date(),
            todaysUtcDate = new Date(now.getUTCFullYear(), now.getUTCMonth(),
                now.getUTCDate(), now.getUTCHours(), now.getUTCMinutes(), now.getUTCSeconds());
        futureDate.setDate(todaysUtcDate.getDate() + parseInt(params[0]));
        if (selectedDate >= futureDate) {
            return false;
        }
        return true;
    });
    // Add Future Date adapter.
    $.validator.unobtrusive.adapters.add('futuredate',
        ['days'],
        function (options) {
            options.rules['futuredate'] = [parseInt(options.params['days'])];
            options.messages['futuredate'] = options.message;
        });


})(jQuery); // end of jQuery name space