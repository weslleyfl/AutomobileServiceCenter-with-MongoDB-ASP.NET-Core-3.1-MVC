(function($){
  $(function(){

    $('.button-collapse').sideNav();
    $('.parallax').parallax();

      // Impedir os botões voltar e avançar do navegador.
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
})(jQuery); // end of jQuery name space