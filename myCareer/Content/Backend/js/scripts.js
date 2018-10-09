/*
 * NYASA ADMIN TEMPLATE JAVASCRIPT
 * ======================================================================
 * NOTE : All JavaScript plugins require jQuery to be included
 * http://jquery.com/
 *
 */
/* ========================================================================
 * SELECTOR CACHE v.1.0
 * -------------------------------------------------------------------------
 * - Designbudy.com -
 * ========================================================================*/
/*
To improve performance and load time, you don't need to create a new variable to get main selector,
for the main selector has been cached and used in all of plugins, just need to call the variables.

Example:
To get selector "#container" maybe you can use

var $container = $ ('# container');
$container.addClass('effect');


For more efficient, simply called "nyasa.container".


nyasa.container.addClass('effect');


Both of the above methods will produce the same results.

*/
$(document).ready(function() {
    "use strict";
    window.jasmine = {
            container: $("#container"),
            contentContainer: $("#content-container"),
            navbar: $("#navbar"),
            mainNav: $("#mainnav-container"),
            aside: $("#aside-container"),
            footer: $("#footer"),
            scrollTop: $("#scroll-top"),
            window: $(window),
            body: $("body"),
            bodyHtml: $("body, html"),
            document: $(document),
            screenSize: "",
            isMobile: function() {
                return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)
            }(),
            randomInt: function(a, b) {
                return Math.floor(Math.random() * (b - a + 1) + a)
            },
            transition: function() {
                var a = document.body || document.documentElement,
                    b = a.style,
                    c = void 0 !== b.transition || void 0 !== b.WebkitTransition;
                return c
            }()
        },

        jasmine.window.on("load", function() {
            //Activate the Bootstrap tooltips		
            var a = $(".add-tooltip");
            a.length && a.tooltip();

            var b = $(".add-popover");
            b.length && b.popover();

            // STYLEABLE SCROLLBARS
            // =================================================================
            // Require nanoScroller
            // http://jamesflorentino.github.io/nanoScrollerJS/
            // =================================================================

            var c = $(".nano");
            c.length && c.nanoScroller({
                    preventPageScrolling: !0
                }),

                // Update nancoscroller		
                $("#navbar-container .navbar-top-links").on("shown.bs.dropdown", ".dropdown", function() {
                    $(this).find(".nano").nanoScroller({
                        preventPageScrolling: !0
                    })
                }),

                jasmine.body.addClass("page-effect")
        }),


        /* ========================================================================
         * PANEL REMOVAL v1.0
         * -------------------------------------------------------------------------
         * Optional Font Icon : By Font Awesome
         * http://fortawesome.github.io/Font-Awesome/
         * ========================================================================*/

        jasmine.window.on("load", function() {
            var a = $('[data-dismiss="panel"]');
            a.length && a.one("click", function(a) {
                a.preventDefault();
                var b = $(this).parents(".panel");
                b.addClass("remove").on("transitionend webkitTransitionEnd oTransitionEnd MSTransitionEnd", function(a) {
                    "opacity" == a.originalEvent.propertyName && b.remove()
                })
            })
        }),

        /* ========================================================================
         * SCROLL TO TOP BUTTON v1.0
         * -------------------------------------------------------------------------
         * Optional Font Icon : By Font Awesome
         * http://fortawesome.github.io/Font-Awesome/
         * ========================================================================*/

        jasmine.window.one("load", function() {
            if (jasmine.scrollTop.length && !jasmine.isMobile) {
                var a = !0,
                    b = 250;

                jasmine.window.scroll(function() {
                        jasmine.window.scrollTop() > b && !a ? (jasmine.navbar.addClass("shadow"), jasmine.scrollTop.addClass("in"), a = !0) : jasmine.window.scrollTop() < b && a && (jasmine.navbar.removeClass("shadow"), jasmine.scrollTop.removeClass("in"), a = !1)
                    }),

                    jasmine.scrollTop.on("click", function(a) {
                        a.preventDefault(), jasmine.bodyHtml.animate({
                            scrollTop: 0
                        }, 500)
                    })
            }
        });


    /* ========================================================================
     * nyasa OVERLAY v1.0
     * -------------------------------------------------------------------------
     * Optional Font Icon : By Font Awesome
     * http://fortawesome.github.io/Font-Awesome/
     * ========================================================================*/


    var a = {
            displayIcon: true,
            iconColor: "text-dark",
            iconClass: "fa fa-refresh fa-spin fa-2x",
            title: "",
            desc: ""
        },

        b = function() {
            return (65536 * (1 + Math.random()) | 0).toString(16).substring(1)
        },

        c = {
            show: function(a) {
                var c = $(a.attr("data-target")),
                    d = "jasmine-overlay-" + b() + b() + "-" + b(),
                    e = $('<div id="' + d + '" class="panel-overlay"></div>');
                return a.prop("disabled", !0).data("jasmineOverlay", d), c.addClass("panel-overlay-wrap"), e.appendTo(c).html(a.data("overlayTemplate")), null
            },
            hide: function(a) {
                var b = $(a.attr("data-target")),
                    c = $("#" + a.data("jasmineOverlay"));
                return c.length && (a.prop("disabled", !1), b.removeClass("panel-overlay-wrap"), c.hide().remove()), null
            }
        },

        d = function(b, c) {
            if (b.data("overlayTemplate")) return null;
            var d = $.extend({}, a, c),
                e = d.displayIcon ? '<span class="panel-overlay-icon ' + d.iconColor + '"><i class="' + d.iconClass + '"></i></span>' : "";
            return b.data("overlayTemplate", '<div class="panel-overlay-content pad-all unselectable">' + e + '<h4 class="panel-overlay-title">' + d.title + "</h4><p>" + d.desc + "</p></div>"), null
        };

    $.fn.jasmineOverlay = function(a) {
        return c[a] ? c[a](this) : "object" != typeof a && a ? null : this.each(function() {
            d($(this), a)
        })
    };

    /* ========================================================================
     * nyasa NOTIFICATION v1.1
     * -------------------------------------------------------------------------
     * By Squaredesigns.net
     * ========================================================================*/

    var e, g, f = {},
        h = !1;
    $.jasmineNoty = function(a) {
        var j, b = {
                type: "primary",
                icon: "",
                title: "",
                message: "",
                closeBtn: true,
                container: "page",
                floating: {
                    position: "top-right",
                    animationIn: "jellyIn",
                    animationOut: "fadeOut"
                },
                html: null,
                focus: true,
                timer: 0
            },

            c = $.extend({}, b, a),
            d = $('<div class="alert-wrap"></div>'),

            i = function() {
                var b = "";
                return a && a.icon && (b = '<div class="media-left"><span class="icon-wrap icon-wrap-xs icon-circle alert-icon"><i class="' + c.icon + '"></i></span></div>'), b
            },

            k = function() {
                var a = c.closeBtn ? '<button class="close" type="button"><i class="fa fa-times-circle"></i></button>' : "",
                    b = '<div class="alert alert-' + c.type + '" role="alert">' + a + '<div class="media">';
                return c.html ? b + c.html + "</div></div>" : b + i() + '<div class="media-body"><h4 class="alert-title">' + c.title + '</h4><p class="alert-message">' + c.message + "</p></div></div>"
            }(),

            l = function(a) {
                return "floating" === c.container && c.floating.animationOut && (d.removeClass(c.floating.animationIn).addClass(c.floating.animationOut), jasmine.transition || d.remove()), d.removeClass("in").on("transitionend webkitTransitionEnd oTransitionEnd MSTransitionEnd", function(a) {
                    "max-height" == a.originalEvent.propertyName && d.remove()
                }), clearInterval(j), null
            },

            m = function(a) {
                jasmine.bodyHtml.animate({
                    scrollTop: a
                }, 300, function() {
                    d.addClass("in")
                })
            };

        (function() {
            if ("page" === c.container) e || (e = $('<div id="page-alert"></div>'), jasmine.contentContainer.prepend(e)), g = e, c.focus && m(0);
            else if ("floating" === c.container) f[c.floating.position] || (f[c.floating.position] = $('<div id="floating-' + c.floating.position + '" class="floating-container"></div>'), jasmine.container.append(f[c.floating.position])), g = f[c.floating.position], c.floating.animationIn && d.addClass("in animated " + c.floating.animationIn), c.focus = !1;
            else {
                var a = $(c.container),
                    b = a.children(".panel-alert"),
                    i = a.children(".panel-heading");
                if (!a.length) return h = !1, !1;
                b.length ? g = b : (g = $('<div class="panel-alert"></div>'), i.length ? i.after(g) : a.prepend(g)), c.focus && m(a.offset().top - 30)
            }
            return h = !0, !1
        })();
        if (h && (g.append(d.html(k)), d.find('[data-dismiss="noty"]').one("click", l), c.closeBtn && d.find(".close").one("click", l), c.timer > 0 && (j = setInterval(l, c.timer)), !c.focus)) var o = setInterval(function() {
            d.addClass("in"), clearInterval(o)
        }, 200)
    };

    /* ========================================================================
     * nyasa CHECK v1.1
     * -------------------------------------------------------------------------
     * - squaredesigns.net -
     * ========================================================================*/

    var i, j = function(a) {
            if (!a.data("jasmine-check")) {
                a.data("jasmine-check", !0), a.text().trim().length ? a.addClass("form-text") : a.removeClass("form-text");
                var b = a.find("input")[0],
                    c = b.name,
                    d = function() {
                        return !("radio" != b.type || !c) && $(".form-radio").not(a).find("input").filter("input[name=" + c + "]").parent()
                    }(),
                    e = function() {
                        "radio" == b.type && d.length && d.each(function() {
                            var a = $(this);
                            a.hasClass("active") && a.trigger("jasmine.ch.unchecked"), a.removeClass("active")
                        }), b.checked ? a.addClass("active").trigger("jasmine.ch.checked") : a.removeClass("active").trigger("jasmine.ch.unchecked")
                    };
                b.checked ? a.addClass("active") : a.removeClass("active"), $(b).on("change", e)
            }
        },

        c = {
            isChecked: function() {
                return this[0].checked
            },
            toggle: function() {
                return this[0].checked = !this[0].checked, this.trigger("change"), null
            },
            toggleOn: function() {
                return this[0].checked || (this[0].checked = !0, this.trigger("change")), null
            },
            toggleOff: function() {
                return this[0].checked && "checkbox" == this[0].type && (this[0].checked = !1, this.trigger("change")), null
            }
        };


    $.fn.jasmineCheck = function(a) {
            var b = !1;
            return this.each(function() {
                c[a] ? b = c[a].apply($(this).find("input"), Array.prototype.slice.call(arguments, 1)) : "object" != typeof a && a || j($(this))
            }), b
        },

        jasmine.document.ready(function() {
            i = $(".form-checkbox, .form-radio"), i.length && i.jasmineCheck()
        }),

        jasmine.document.on("change", ".btn-file :file", function() {
            var a = $(this),
                b = a.get(0).files ? a.get(0).files.length : 1,
                c = a.val().replace(/\\/g, "/").replace(/.*\//, ""),
                d = function() {
                    try {
                        return a[0].files[0].size
                    } catch (a) {
                        return "Nan"
                    }
                }(),
                e = function() {
                    if ("Nan" == d) return "Unknown";
                    var a = Math.floor(Math.log(d) / Math.log(1024));
                    return 1 * (d / Math.pow(1024, a)).toFixed(2) + " " + ["B", "kB", "MB", "GB", "TB"][a]
                }();
            a.trigger("fileselect", [b, c, e])
        }),

        // NAVIGATION SHORTCUT BUTTONS
        // =================================================================
        // Require Bootstrap Popover
        // http://getbootstrap.com/javascript/#popovers
        // =================================================================

        jasmine.window.on("load", function() {
            var a = $("#mainnav-shortcut");
            a.length && a.find("li").each(function() {
                var a = $(this);
                a.popover({
                    animation: !1,
                    trigger: "hover focus",
                    placement: "bottom",
                    container: "#mainnav-container",
                    template: '<div class="popover mainnav-shortcut"><div class="arrow"></div><div class="popover-content"></div>'
                })
            })
        });

    /* ========================================================================
     * nyasa NAVIGATION v1.2.1
     * -------------------------------------------------------------------------
     *
     * Require Bootstrap Popover
     * http://getbootstrap.com/javascript/#popovers
     *
     * Require jQuery Resize End
     * https://github.com/nielse63/jQuery-ResizeEnd
     *
     * ========================================================================*/

    var k = $('#mainnav-menu > li > a, #mainnav-menu-wrap .mainnav-widget a[data-toggle="menu-widget"]'),
        l = $("#mainnav").height(),
        m = null,
        n = false,
        o = false,
        p = null,

        // Determine and bind hover or "touch" event
        // ===============================================

        r = function() {
            var a;
            k.each(function() {
                var b = $(this),
                    c = b.children(".menu-title"),
                    d = b.siblings(".collapse"),
                    e = $(b.attr("data-target")),
                    f = e.length ? e.parent() : null,
                    g = null,
                    h = null,
                    i = null,
                    j = null,
                    p = (b.outerHeight() - b.height() / 4, function() {
                        return e.length && b.on("click", function(a) {
                            a.preventDefault()
                        }), !!d.length && (b.on("click", function(a) {
                            a.preventDefault()
                        }).parent("li").removeClass("active"), !0)
                    }()),
                    q = null,
                    r = function(a) {
                        clearInterval(q), q = setInterval(function() {
                            a.nanoScroller({
                                preventPageScrolling: !0,
                                alwaysVisible: !0
                            }), clearInterval(q)
                        }, 700)
                    };


                $(document).click(function(a) {
                        $(a.target).closest("#mainnav-container").length || b.removeClass("hover").popover("hide")
                    }),

                    $("#mainnav-menu-wrap > .nano").on("update", function(a, c) {
                        b.removeClass("hover").popover("hide")
                    }),

                    b.popover({
                        animation: false,
                        trigger: "manual",
                        container: "#mainnav",
                        viewport: b,
                        html: true,
                        title: function() {
                            return p ? c.html() : null
                        },
                        content: function() {
                            var a;
                            return p ? (a = $('<div class="sub-menu"></div>'), d.addClass("pop-in").wrap('<div class="nano-content"></div>').parent().appendTo(a)) : e.length ? (a = $('<div class="sidebar-widget-popover"></div>'), e.wrap('<div class="nano-content"></div>').parent().appendTo(a)) : a = '<span class="single-content">' + c.html() + "</span>", a
                        },
                        template: '<div class="popover menu-popover"><h4 class="popover-title"></h4><div class="popover-content"></div></div>'
                    }).on("show.bs.popover", function() {
                        if (!g) {
                            if (g = b.data("bs.popover").tip(), h = g.find(".popover-title"), i = g.children(".popover-content"), !p && 0 == e.length) return;
                            j = i.children(".sub-menu")
                        }!p && 0 == e.length
                    }).on("shown.bs.popover", function() {
                        if (!p && 0 == e.length) {
                            var a = 0 - .5 * b.outerHeight();
                            return void i.css({
                                "margin-top": a + "px",
                                width: "auto"
                            })
                        }

                        var c = parseInt(g.css("top")),
                            d = b.outerHeight(),
                            f = function() {
                                return jasmine.container.hasClass("mainnav-fixed") ? $(window).outerHeight() - c - d : $(document).height() - c - d
                            }(),
                            j = i.find(".nano-content").children().css("height", "auto").outerHeight();
                        i.find(".nano-content").children().css("height", ""), c > f ? (h.length && !h.is(":visible") && (d = Math.round(0 - .5 * d)), c -= 5, i.css({
                                top: "",
                                bottom: d + "px",
                                height: c
                            })

                            .children().addClass("nano").css({
                                width: "100%"
                            })

                            .nanoScroller({
                                preventPageScrolling: true
                            }),

                            r(i.find(".nano"))) : (!jasmine.container.hasClass("navbar-fixed") && jasmine.mainNav.hasClass("affix-top") && (f -= 50), j > f ? ((jasmine.container.hasClass("navbar-fixed") || jasmine.mainNav.hasClass("affix-top")) && (f -= d + 5), f -= 5, i.css({
                                top: d + "px",
                                bottom: "",
                                height: f
                            })

                            .children().addClass("nano").css({
                                width: "100%"
                            })

                            .nanoScroller({
                                preventPageScrolling: true
                            }), r(i.find(".nano"))) : (h.length && !h.is(":visible") && (d = Math.round(0 - .5 * d)), i.css({
                            top: d + "px",
                            bottom: "",
                            height: "auto"
                        }))), h.length && h.css("height", b.outerHeight()), i.on("click", function() {
                            i.find(".nano-pane").hide(), r(i.find(".nano"))
                        })
                    }).on("hidden.bs.popover", function() {
                        b.removeClass("hover"), p ? d.removeAttr("style").appendTo(b.parent()) : e.length && e.appendTo(f), clearInterval(a)
                    }).on("click", function() {
                        jasmine.container.hasClass("mainnav-sm") && (k.popover("hide"), b.addClass("hover").popover("show"))
                    }).hover(function() {
                        k.popover("hide"), b.addClass("hover").popover("show")
                    }, function() {
                        clearInterval(a), a = setInterval(function() {
                            g && (g.one("mouseleave", function() {
                                b.removeClass("hover").popover("hide")
                            }), g.is(":hover") || b.removeClass("hover").popover("hide")), clearInterval(a)
                        }, 500)
                    })
            }), o = !0
        },
        s = function() {
            var a = $("#mainnav-menu").find(".collapse");
            a.length && a.each(function() {
                var a = $(this);
                a.hasClass("in") ? a.parent("li").addClass("active") : a.parent("li").removeClass("active")
            }), null != m && m.length && m.nanoScroller({
                stop: !0
            }), k.popover("destroy").unbind("mouseenter mouseleave"), o = !1
        },
        t = function() {
            var b, a = jasmine.container.width();
            b = a <= 740 ? "xs" : a > 740 && a < 992 ? "sm" : a >= 992 && a <= 1200 ? "md" : "lg", p != b && (p = b, jasmine.screenSize = b, "sm" == jasmine.screenSize && jasmine.container.hasClass("mainnav-lg") && $.jasmineNav("collapse"))
        },
        u = function(a) {
            return jasmine.mainNav.jasmineAffix("update"), s(), t(), ("collapse" == n || jasmine.container.hasClass("mainnav-sm")) && (jasmine.container.removeClass("mainnav-in mainnav-out mainnav-lg"), r()), l = $("#mainnav").height(), n = !1, null
        },
        c = {
            revealToggle: function() {
                jasmine.container.hasClass("reveal") || jasmine.container.addClass("reveal"), jasmine.container.toggleClass("mainnav-in mainnav-out").removeClass("mainnav-lg mainnav-sm"), o && s()
            },
            revealIn: function() {
                jasmine.container.hasClass("reveal") || jasmine.container.addClass("reveal"), jasmine.container.addClass("mainnav-in").removeClass("mainnav-out mainnav-lg mainnav-sm"), o && s()
            },
            revealOut: function() {
                jasmine.container.hasClass("reveal") || jasmine.container.addClass("reveal"), jasmine.container.removeClass("mainnav-in mainnav-lg mainnav-sm").addClass("mainnav-out"), o && s()
            },
            slideToggle: function() {
                jasmine.container.hasClass("slide") || jasmine.container.addClass("slide"), jasmine.container.toggleClass("mainnav-in mainnav-out").removeClass("mainnav-lg mainnav-sm"), o && s()
            },
            slideIn: function() {
                jasmine.container.hasClass("slide") || jasmine.container.addClass("slide"), jasmine.container.addClass("mainnav-in").removeClass("mainnav-out mainnav-lg mainnav-sm"), o && s()
            },
            slideOut: function() {
                jasmine.container.hasClass("slide") || jasmine.container.addClass("slide"), jasmine.container.removeClass("mainnav-in mainnav-lg mainnav-sm").addClass("mainnav-out"), o && s()
            },
            pushToggle: function() {
                jasmine.container.toggleClass("mainnav-in mainnav-out").removeClass("mainnav-lg mainnav-sm"), jasmine.container.hasClass("mainnav-in mainnav-out") && jasmine.container.removeClass("mainnav-in"), o && s()
            },
            pushIn: function() {
                jasmine.container.addClass("mainnav-in").removeClass("mainnav-out mainnav-lg mainnav-sm"), o && s()
            },
            pushOut: function() {
                jasmine.container.removeClass("mainnav-in mainnav-lg mainnav-sm").addClass("mainnav-out"), o && s()
            },
            colExpToggle: function() {
                return jasmine.container.hasClass("mainnav-lg mainnav-sm") && jasmine.container.removeClass("mainnav-lg"), jasmine.container.toggleClass("mainnav-lg mainnav-sm").removeClass("mainnav-in mainnav-out"), jasmine.window.trigger("resize")
            },
            collapse: function() {
                return jasmine.container.addClass("mainnav-sm").removeClass("mainnav-lg mainnav-in mainnav-out"), n = "collapse", jasmine.window.trigger("resize")
            },
            expand: function() {
                return jasmine.container.removeClass("mainnav-sm mainnav-in mainnav-out").addClass("mainnav-lg"), jasmine.window.trigger("resize")
            },
            togglePosition: function() {
                jasmine.container.toggleClass("mainnav-fixed"), jasmine.mainNav.jasmineAffix("update")
            },
            fixedPosition: function() {
                jasmine.container.addClass("mainnav-fixed"), jasmine.mainNav.jasmineAffix("update")
            },
            staticPosition: function() {
                jasmine.container.removeClass("mainnav-fixed"), jasmine.mainNav.jasmineAffix("update")
            },
            update: u,
            forceUpdate: t,
            getScreenSize: function() {
                return p
            }
        };

    $.jasmineNav = function(a, b) {
            if (c[a]) {
                "colExpToggle" != a && "expand" != a && "collapse" != a || ("xs" == jasmine.screenSize && "collapse" == a ? a = "pushOut" : "xs" != jasmine.screenSize && "sm" != jasmine.screenSize || "colExpToggle" != a && "expand" != a || !jasmine.container.hasClass("mainnav-sm") || (a = "pushIn"));
                var d = c[a].apply(this, Array.prototype.slice.call(arguments, 1));
                if (b) return b();
                if (d) return d
            }
            return null
        },

        $.fn.isOnScreen = function() {
            var a = {
                top: jasmine.window.scrollTop(),
                left: jasmine.window.scrollLeft()
            };
            a.right = a.left + jasmine.window.width(), a.bottom = a.top + jasmine.window.height();
            var b = this.offset();
            return b.right = b.left + this.outerWidth(), b.bottom = b.top + this.outerHeight(), !(a.right < b.left || a.left > b.right || a.bottom < b.bottom || a.top > b.top)
        },

        jasmine.window.on("resizeEnd", u).trigger("resize"), jasmine.window.on("load", function() {

            var a = $(".mainnav-toggle");
            a.length && a.on("click", function(b) {
                b.preventDefault(), a.hasClass("push") ? $.jasmineNav("pushToggle") : a.hasClass("slide") ? $.jasmineNav("slideToggle") : a.hasClass("reveal") ? $.jasmineNav("revealToggle") : $.jasmineNav("colExpToggle")
            });


            // COLLAPSIBLE MENU LIST
            // =================================================================
            // Require MetisMenu
            // http://demo.onokumus.com/metisMenu/
            // =================================================================

            var b = $("#mainnav-menu");
            b.length && ($("#mainnav-menu").metisMenu({
                    toggle: true
                }),

                // STYLEABLE SCROLLBARS
                // =================================================================
                // Require nanoScroller
                // http://jamesflorentino.github.io/nanoScrollerJS/
                // =================================================================

                m = jasmine.mainNav.find(".nano"), m.length && m.nanoScroller({
                    preventPageScrolling: true
                }))
        });

    /* ========================================================================
     * nyasa ASIDE v1.0.1
     * -------------------------------------------------------------------------
     * - squaredesigns.net -
     * ========================================================================*/

    var w = {
            toggleHideShow: function() {
                jasmine.container.toggleClass("aside-in"), jasmine.window.trigger("resize"), jasmine.container.hasClass("aside-in") && x()
            },
            show: function() {
                jasmine.container.addClass("aside-in"), jasmine.window.trigger("resize"), x()
            },
            hide: function() {
                jasmine.container.removeClass("aside-in"), jasmine.window.trigger("resize")
            },
            toggleAlign: function() {
                jasmine.container.toggleClass("aside-left"), jasmine.aside.jasmineAffix("update")
            },
            alignLeft: function() {
                jasmine.container.addClass("aside-left"), jasmine.aside.jasmineAffix("update")
            },
            alignRight: function() {
                jasmine.container.removeClass("aside-left"), jasmine.aside.jasmineAffix("update")
            },
            togglePosition: function() {
                jasmine.container.toggleClass("aside-fixed"), jasmine.aside.jasmineAffix("update")
            },
            fixedPosition: function() {
                jasmine.container.addClass("aside-fixed"), jasmine.aside.jasmineAffix("update")
            },
            staticPosition: function() {
                jasmine.container.removeClass("aside-fixed"), jasmine.aside.jasmineAffix("update")
            },
            toggleTheme: function() {
                jasmine.container.toggleClass("aside-bright")
            },
            brightTheme: function() {
                jasmine.container.addClass("aside-bright")
            },
            darkTheme: function() {
                jasmine.container.removeClass("aside-bright")
            }
        },
        x = function() {
            jasmine.container.hasClass("mainnav-in") && "xs" != jasmine.screenSize && ("sm" == jasmine.screenSize ? $.jasmineNav("collapse") : jasmine.container.removeClass("mainnav-in mainnav-lg mainnav-sm").addClass("mainnav-out"))
        };
    $.jasmineAside = function(a, b) {
            return w[a] && (w[a].apply(this, Array.prototype.slice.call(arguments, 1)), b) ? b() : null
        },

        jasmine.window.on("load", function() {
            if (jasmine.aside.length) {

                // STYLEABLE SCROLLBARS
                // =================================================================
                // Require nanoScroller
                // http://jamesflorentino.github.io/nanoScrollerJS/
                // =================================================================

                jasmine.aside.find(".nano").nanoScroller({
                    preventPageScrolling: !0,
                    alwaysVisible: !1
                });

                var a = $(".aside-toggle");
                a.length && a.on("click", function(a) {
                    $.jasmineAside("toggleHideShow")
                })
            }
        }),

        /* ========================================================================
         * nyasa AFFIX v1.0
         * -------------------------------------------------------------------------
         * Require Bootstrap Affix
         * http://getbootstrap.com/javascript/#affix
         * ========================================================================*/


        $.fn.jasmineAffix = function(a) {
            return this.each(function() {
                var c, b = $(this);
                "object" != typeof a && a ? "update" == a && (c = b.data("jasmine.af.class")) : (c = a.className, b.data("jasmine.af.class", a.className)), jasmine.container.hasClass(c) && !jasmine.container.hasClass("navbar-fixed") ? b.affix({
                    offset: {
                        top: $("#navbar").outerHeight()
                    }
                }) : jasmine.container.hasClass(c) && !jasmine.container.hasClass("navbar-fixed") || (jasmine.window.off(b.attr("id") + ".affix"), b.removeClass("affix affix-top affix-bottom").removeData("bs.affix"))
            })
        },

        jasmine.window.on("load", function() {
            jasmine.mainNav.length && jasmine.mainNav.jasmineAffix({
                className: "mainnav-fixed"
            }), jasmine.aside.length && jasmine.aside.jasmineAffix({
                className: "aside-fixed"
            })
        }),

        $(".inbox-star").click(function() {
            $(this).toggleClass("starred")
        }),

        $("#profilebtn").click(function() {
            $("#profilebody").slideToggle()
        }),

        $(".conversation-toggle").on("click", function() {
            $(".conversation").toggleClass("closed")
        });

    var y = Array.prototype.slice.call(document.querySelectorAll(".demo-switch"));
    y.forEach(function(a) {
            new Switchery(a)
        }),

        // ASIDE
        // =================================================================
        // Toggle Right Sidebar
        // =================================================================

        $("#demo-toggle-aside").on("click", function(a) {
            a.preventDefault(), jasmine.container.hasClass("aside-in") ? ($.jasmineAside("hide"), asdVisCheckbox.jasmineCheck("toggleOff")) : ($.jasmineAside("show"), asdVisCheckbox.jasmineCheck("toggleOn"))
        }),

        // FULLSCREEN
        // =================================================================
        // Toggle fullscreen
        // =================================================================


        // =========================================================================
        // CHAT MESSAGE
        // =========================================================================
        // Initialize app when document is ready

        $("#sidebarChatMessage").keydown(function(a) {
            var b = $(a.currentTarget);
            if (13 === a.keyCode) {
                a.preventDefault();
                var c = (new Date).getHours() + ":" + (new Date).getMinutes(),
                    d = $(".msg_container .avatar img").attr("src"),
                    e = "";
                e += '\t<div class="msg_container base_sent">', e += '\t  <div class="col-md-9 col-xs-9">', e += '\t    <div class="messages msg_sent">', e += "\t\t\t" + b.val(), e += "\t\t\t<small>" + c + "</small>", e += "\t    </div>", e += "\t  </div>", e += '\t  <div class="col-md-3 col-xs-3 avatar">', e += '\t\t <img class="img-circle" src="' + d + '" alt="">', e += "     </div>", e += "   </div>";
                var f = $(e).hide();
                $(".msg_container_base").prepend(f), f.show("fast")
            }
        })
}), ! function(a, b) {
    var c = {};
    c.eventName = "resizeEnd", c.delay = 250, c.poll = function() {
        var d = a(this),
            e = d.data(c.eventName);
        e.timeoutId && b.clearTimeout(e.timeoutId), e.timeoutId = b.setTimeout(function() {
            d.trigger(c.eventName)
        }, c.delay)
    }, a.event.special[c.eventName] = {
        setup: function() {
            var b = a(this);
            b.data(c.eventName, {}), b.on("resize", c.poll)
        },
        teardown: function() {
            var d = a(this),
                e = d.data(c.eventName);
            e.timeoutId && b.clearTimeout(e.timeoutId), d.removeData(c.eventName), d.off("resize", c.poll)
        }
    }, a.fn[c.eventName] = function(a, b) {
        return arguments.length > 0 ? this.on(c.eventName, null, a, b) : this.trigger(c.eventName)
    }
}(jQuery, this);