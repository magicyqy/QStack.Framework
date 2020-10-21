
; (function ($) {
    __cacheAjaxList = [];
    $.extend({
        addCacheAjax: function (elOrFunc) {
            __cacheAjaxList.push(elOrFunc);
        },
        callCacheAjax: function () {
            var lastAjax = __cacheAjaxList.pop();
            //console.log(typeof lastAjax);
            if (lastAjax instanceof Function)
                lastAjax();
            else if (lastAjax instanceof jQuery)
                $(lastAjax).click();
        },
        removeCacheAjax: function () {
            __cacheAjaxList.pop();
        }
    });
    $.fn.serializeObject = function () {
        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name]) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };
    $.ajaxSetup({
        error: function (jqXHR, textStatus, errorThrown) {
            if (jqXHR.status == 428) {
                if (jqXHR.responseJSON && jqXHR.responseJSON.code === 40300) {
                    var url = jqXHR.responseJSON.data;
                    $("#bt-modal").modal("toggle");
                    $.get(url, function (response) {
                        if (response) {
                            $("#bt-modal-dialog .modal-content").html(response);
                        }
                    });
                }
                return;
            }
        }
    });

    // 搜索
    $(".fa-search").click(function () {
        $("#main-search").slideToggle(500);
    });
    $(".fa-money").click(function () {
        $("#contribute").slideToggle(100);
    });
    // 左侧滚动条
    $('.lyear-header-container').niceScroll();

    // 导航切换
    $('body').on('click', '.lyear-hamburger', function () {
        $(this).toggleClass("is-active");
    });

    // 评论留言
    $('.reply-btn').on('click', function () {
        var parentLi = $(this).parents('li').first(),
            parentID = $(this).data('id'),
            respond = $('#respond'),
            respondHtml = respond.prop("outerHTML");

        $("#respond").remove();
        parentLi.after(respondHtml);
        $("#comment_parent").val(parentID);
        $('.cancel-comment-reply').show();
    });

    //* Navbar Fixed  
    function navbarFixed() {
        if ($('.main_header_area').length) {
            $(window).on('scroll', function () {
                var scroll = $(window).scrollTop();
                if (scroll >= 20) {
                    $("#header").addClass("navbar_fixed");
                    $("#main-search").addClass("navbar_fixed");
                } else {
                    $("#header").removeClass("navbar_fixed");
                    $("#main-search").removeClass("navbar_fixed");
                }
            });
        };
    };

    // Scroll to top
    function scrollToTop() {
        if ($('.scroll-top').length) {
            $(window).on('scroll', function () {
                if ($(this).scrollTop() > 200) {
                    $('.scroll-top').fadeIn();
                } else {
                    $('.scroll-top').fadeOut();
                }
            });
            //Click event to scroll to top
            $('.scroll-top').on('click', function () {
                $('html, body').animate({
                    scrollTop: 0
                }, 1000);
                return false;
            });
        }
    }

    // Product value
    function productValue() {
        var inputVal = $("#product-value");
        if (inputVal.length) {
            $('#value-decrease').on('click', function () {
                inputVal.html(function (i, val) {
                    return val * 1 - 1
                });
            });
            $('#value-increase').on('click', function () {
                inputVal.html(function (i, val) {
                    return val * 1 + 1
                });
            });
        }
    }

    //* Select js
    function nice_Select() {
        if ($('.post_select').length) {
            $('select').niceSelect();
        };
    };

    // Preloader JS 
    var window_loaded = false;
    var preloader_timeout = 10000;
    function preloader() {
        if ($('.preloader').length) {
            $(window).on('load', function () {
                window_loaded = true;
                $('.preloader').fadeOut();
                $('.preloader').delay(50).fadeOut('slow');
            })
            setTimeout(function () {
                if (!window_loaded) {
                    $('.preloader').fadeOut();
                    $('.preloader').delay(50).fadeOut('slow');
                }
            }, preloader_timeout);
        }
    }

    /*Function Calls*/
    new WOW().init();
    navbarFixed();
    scrollToTop();
    nice_Select();
    productValue();
    preloader();
})(jQuery);

function isEmailAvailable(emailInput) {

    var myreg = /^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/;
    if (!myreg.test(emailInput)) {
        return false;
    }
    else {
        return true;
    }
}
// 获取指定范围内的随机数
function randomAccess(min, max) {
    return Math.floor(Math.random() * (min - max) + max)
}

// 解码
function decodeUnicode(str) {
    //Unicode显示方式是\u4e00
    str = "\\u" + str
    str = str.replace(/\\/g, "%");
    //转换中文
    str = unescape(str);
    //将其他受影响的转换回原来
    str = str.replace(/%/g, "\\");
    return str;
}

/*
*@param Number NameLength 要获取的名字长度
*/
function getRandomName(NameLength) {
    let name = ""
    for (let i = 0; i < NameLength; i++) {
        let unicodeNum = ""
        unicodeNum = randomAccess(0x4e00, 0x9fa5).toString(16)
        name += decodeUnicode(unicodeNum)
    }
    return name
}

function setLocalStorageItem(name, value) {
    if (window.localStorage) {
        window.localStorage.setItem(name, value);
    }
}
function getLocalStorageItem(name) {
    if (window.localStorage) {
        return window.localStorage.getItem(name);
    }
}