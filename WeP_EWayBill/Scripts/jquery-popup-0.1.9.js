var OpenLayerParams = {
    ID: "OpenLayerAccessID",
    FadeSpeed: 350,
    StartTop: 0
};

//url, frmw, frmh, IsScroll, callback, closeCallback, roundPadding
$.fn.openLayer = function (params) {
	if (params.url != null && params.url != "") {
        if (document.getElementById(OpenLayerParams.ID) == null) {
            var dh = $(document).outerHeight(true);
            var wh = $(window).outerHeight(true);
            var h = (dh > wh) ? dh : wh;

            var dw = $(document).outerWidth(true);
            var ww = $(window).outerWidth(true);
            var w = (dw > ww) ? dw : ww;
            var posX = (w / 2) - ((params.frmw / 2) + 20);
            var posY = (wh / 2) - (params.frmh / 2);
            posY = posY * 0.7;

            if ((params.frmh + posY) > wh) {
                posY = posY - ((params.frmh + posY) - wh);
            }

            if (posY < 10) {
                posY = 10;
            }

            OpenLayerParams.StartTop = posY;

            if ($(document).scrollTop() > 0) {
                posY = posY + $(document).scrollTop();
            }

            var baseClass = "PopupLayer";
            var basePadding = 20;
            if (params.roundPadding == false) {
                baseClass = "PopupLayerNoPadding";
                basePadding = 0;
            }

            var Tags = "<div id=\"" + OpenLayerParams.ID + "\" class=\"" + baseClass + "\" style=\"height:" + String(h) + "px\">";
            Tags += "<div id=\"" + OpenLayerParams.ID + "_Foreground\" class=\"PopupLayer_Fore\" style=\"left:" + posX + "px;top:" + posY + "px;width:" + String(params.frmw + basePadding) + "px;height:" + String(params.frmh + basePadding) + "px;\">";
            Tags += "<div class=\"PopupLayer_Btn\"><img class=\"closeLayerBtn\" src=\"/Content/img/close_btn.png\" /></div>";
            Tags += "<iframe id=\"" + OpenLayerParams.ID + "_frm\" name=\"" + OpenLayerParams.ID + "_frm\" frameborder=\"0\" width=\"" + String(params.frmw) + "px\" ";
            if (params.IsScroll) {
                Tags += " class=\"onScroll\" scrollbar=\"yes\" ";
            } else {
                Tags += " class=\"offScroll\" scrollbar=\"no\" ";
            }
            Tags += " height=\"" + String(params.frmh) + "px\" src=\"" + params.url + "\"></iframe>";
            Tags += "</div>";
            Tags += "<div id=\"" + OpenLayerParams.ID + "_Background\" class=\"PopupLayer_Back\" style=\"height:" + String(h) + "px\">";
            Tags += "</div>";
            Tags += "</div>";

            $("body").prepend(Tags);
            if (!params.IsScroll) {
                $("#" + OpenLayerParams.ID + "_frm").bind("load", function () {
                    $("#" + OpenLayerParams.ID + "_frm").contents().find("body").addClass('hideBody');
                });
            }
            $("#" + OpenLayerParams.ID).fadeIn(OpenLayerParams.FadeSpeed, function () {
				$(".closeLayerBtn").on({
					"click" : function() {
						$(this).closeLayer();
					}
				});
				
                if (params.callback != null) {
                    params.callback();
                }
            });
        }
    } else {
        alert("오픈할 대상을 지정해 주세요.");
    }
};

$.fn.closeLayer = function () {
    if (document.getElementById(OpenLayerParams.ID) != null) {
        $("#" + OpenLayerParams.ID).remove();
    }
};

$(window).on({
	"scroll": function() {
		if (document.getElementById(OpenLayerParams.ID + "_Foreground") != null) {
			$("#" + OpenLayerParams.ID + "_Foreground").css("top", String(OpenLayerParams.StartTop + $(document).scrollTop()) + "px");
		}
	}
});