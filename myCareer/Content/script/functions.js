jQuery(document).ready(function($) {
	   
     'use strict';
     
  	//*** Function Banner
  	jQuery('.careerfy-testimonial-slider').slick({
        slidesToShow: 1,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 5000,
        infinite: true,
        dots: false,
        prevArrow: "<span class='slick-arrow-left'><i class='careerfy-icon careerfy-arrows4'></i></span>",
        nextArrow: "<span class='slick-arrow-right'><i class='careerfy-icon careerfy-arrows4'></i></span>",
        responsive: [
          {
            breakpoint: 1024,
            settings: {
              slidesToShow: 1,
              slidesToScroll: 1,
              infinite: true,
            }
          },
          {
            breakpoint: 800,
            settings: {
              slidesToShow: 1,
              slidesToScroll: 1
            }
          },
          {
            breakpoint: 400,
            settings: {
              slidesToShow: 1,
              slidesToScroll: 1
            }
          }
        ]
      });

    //*** Function Services Slider
    jQuery('.careerfy-service-slider').slick({
        slidesToShow: 5,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 5000,
        infinite: true,
        dots: false,
        centerMode: true,
        centerPadding: '0px',
        prevArrow: "<span class='slick-arrow-left'><i class='careerfy-icon careerfy-arrows4'></i></span>",
        nextArrow: "<span class='slick-arrow-right'><i class='careerfy-icon careerfy-arrows4'></i></span>",
        responsive: [
          {
            breakpoint: 1024,
            settings: {
              slidesToShow: 3,
              slidesToScroll: 1,
              infinite: true,
            }
          },
          {
            breakpoint: 800,
            settings: {
              slidesToShow: 2,
              slidesToScroll: 1
            }
          },
          {
            breakpoint: 400,
            settings: {
              slidesToShow: 1,
              slidesToScroll: 1
            }
          }
        ]
      });

    //*** Function Partner Slider
    jQuery('.careerfy-partner-slider').slick({
        slidesToShow: 6,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 5000,
        infinite: true,
        dots: false,
        centerMode: true,
        centerPadding: '0px',
        arrows: false,
        responsive: [
          {
            breakpoint: 1024,
            settings: {
              slidesToShow: 4,
              slidesToScroll: 1,
              infinite: true,
            }
          },
          {
            breakpoint: 800,
            settings: {
              slidesToShow: 3,
              slidesToScroll: 1
            }
          },
          {
            breakpoint: 400,
            settings: {
              slidesToShow: 1,
              slidesToScroll: 1
            }
          }
        ]
      });

    //*** Function Partner Slider
    jQuery('.careerfy-partnertwo-slider').slick({
        slidesToShow: 6,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 5000,
        infinite: true,
        dots: false,
        prevArrow: "<span class='slick-arrow-left'><i class='careerfy-icon careerfy-arrow-pointing-to-left'></i></span>",
        nextArrow: "<span class='slick-arrow-right'><i class='careerfy-icon careerfy-arrow-pointing-to-right'></i></span>",
        responsive: [
          {
            breakpoint: 1024,
            settings: {
              slidesToShow: 4,
              slidesToScroll: 1,
              infinite: true,
            }
          },
          {
            breakpoint: 800,
            settings: {
              slidesToShow: 3,
              slidesToScroll: 1
            }
          },
          {
            breakpoint: 400,
            settings: {
              slidesToShow: 1,
              slidesToScroll: 1
            }
          }
        ]
      });

    //*** Function Partner Slider
    jQuery('.careerfy-testimonial-styletwo').slick({
        slidesToShow: 2,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 5000,
        infinite: true,
        dots: true,
        prevArrow: "<span class='slick-arrow-left'><i class='careerfy-icon careerfy-arrows22'></i></span>",
        nextArrow: "<span class='slick-arrow-right'><i class='careerfy-icon careerfy-arrows22'></i></span>",
        responsive: [
          {
            breakpoint: 1024,
            settings: {
              slidesToShow: 1,
              slidesToScroll: 1,
              infinite: true,
            }
          },
          {
            breakpoint: 800,
            settings: {
              slidesToShow: 1,
              slidesToScroll: 1
            }
          },
          {
            breakpoint: 400,
            settings: {
              slidesToShow: 1,
              slidesToScroll: 1
            }
          }
        ]
      });

    //*** Function Partner Slider
    jQuery('.careerfy-testimonial-slider-style3').slick({
        slidesToShow: 1,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 5000,
        infinite: true,
        dots: true,
        fade: true,
        prevArrow: $('.careerfy-prev'),
        nextArrow: $('.careerfy-next'),
        responsive: [
          {
            breakpoint: 1024,
            settings: {
              slidesToShow: 1,
              slidesToScroll: 1,
              infinite: true,
            }
          },
          {
            breakpoint: 800,
            settings: {
              slidesToShow: 1,
              slidesToScroll: 1
            }
          },
          {
            breakpoint: 400,
            settings: {
              slidesToShow: 1,
              slidesToScroll: 1
            }
          }
        ]
      });

    //*** Function Partner Slider
    jQuery('.careerfy-partner-style3').slick({
        slidesToShow: 6,
        slidesToScroll: 1,
        autoplay: true,
        autoplaySpeed: 5000,
        infinite: true,
        dots: false,
        arrows: false,
        responsive: [
          {
            breakpoint: 1024,
            settings: {
              slidesToShow: 1,
              slidesToScroll: 1,
              infinite: true,
            }
          },
          {
            breakpoint: 800,
            settings: {
              slidesToShow: 1,
              slidesToScroll: 1
            }
          },
          {
            breakpoint: 400,
            settings: {
              slidesToShow: 1,
              slidesToScroll: 1
            }
          }
        ]
      });

    //***************************
    // Parent AddClass Function
    //***************************
    jQuery(".navbar-nav .sub-menu").parent("li").addClass("submenu-addicon");

});
