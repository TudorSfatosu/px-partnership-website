$(document).ready(function(){
    $( ".special-ul-hover a" ).mouseleave(function() {
        $( this ).parent().addClass( 'special-ul-hover--leave' );
    });
});

var slideIndex = 1;
showSlides(slideIndex);

function plusSlides(n) {
    showSlides(slideIndex += n);
}

function currentSlide(n) {
    showSlides(slideIndex = n);
}

function showSlides(n) {
    var i;
    var slides = document.getElementsByClassName("bannerSlides");
    var dots = document.getElementsByClassName("bannerDot");
    if (n > slides.length) {slideIndex = 1}
    if (n < 1) {slideIndex = slides.length}
    for (i = 0; i < slides.length; i++) {
        slides[i].style.display = "none";
        slides[i].style.opacity = "0";
    }
    for (i = 0; i < dots.length; i++) {
        dots[i].className = dots[i].className.replace(" active", "");
    }
    if(slides.length > 0) {
        slides[slideIndex-1].style.display = "flex";
        slides[slideIndex-1].style.opacity = "1";
    }
    if(dots.length > 0) {
        dots[slideIndex-1].className += " active";
    }
}

var backButton = document.querySelector('.back-button')

function backAnim(){
    if (backButton.classList.contains('back')){
        backButton.classList.remove('back');
    }
    else{
        backButton.classList.add('back');
        setTimeout(backAnim, 1000);
    }
}

jQuery(function($) {
    $('.double-click-open').dblclick(function() {
        return false;
    }).dblclick(function() {
        window.location = this.href;
        return false;
    });
});

$(function () {
	$('.qa-box').matchHeight();
});