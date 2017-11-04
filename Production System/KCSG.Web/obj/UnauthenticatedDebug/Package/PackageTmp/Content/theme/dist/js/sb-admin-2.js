$(function() {

    $('#side-menu').metisMenu();

});

//Loads the correct sidebar on window load,
//collapses the sidebar on window resize.
// Sets the min-height of #page-wrapper to window size
$(function () {
    $(".sub-menu ").on("click", "li", function (event) {
      
    });
  
    $("#menucontent li").removeClass("active");
    var url = window.location;
    var element = $('ul.sub-menu a').filter(function () {
        return this.href == url || url.href.indexOf(this.href) == 0;
    }).addClass('active').parent().addClass('active').parent().addClass('in').prev().addClass("active").parent();

    if (element.is('li')) {
        element.addClass('active');
    }
});
