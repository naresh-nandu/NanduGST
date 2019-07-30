$.fn.loading = function (IsCeate) {
	var LoadingImage = "/Content/img/loading_flower.gif"; //29x29

	if (IsCeate) {
	    if ($(this).selector == "" || String($(this).selector).trim().toLowerCase() == "body") {
	        var dh = $(document).outerHeight(true);
	        var wh = $(window).outerHeight(true);
	        var h = (dh > wh) ? dh : wh;

	        var dw = $(document).outerWidth(true);
	        var ww = $(window).outerWidth(true);
	        var w = (dw > ww) ? dw : ww;

	        var posX = (w / 2) - 34;
	        var posY = (h / 2) - 84;

	        if (document.getElementById("LoadingDivLayer") == null) {
	            var loadingTag = "<div id=\"LoadingDivLayer\" class=\"LoadingDivLayerClass\" style=\"height:" + String(h) + "px\">";
	            loadingTag += "<div class=\"LoadingForeground\" style=\"height:" + String(h) + "px;\">";
	            loadingTag += "<div class=\"LoadingBox\" style=\"top:" + String(posY) + "px;left:" + String(posX) + "px;\">";
	            loadingTag += "<img src=\"" + LoadingImage + "\" alt=\"Loading...\" />";
	            loadingTag += "</div>";
	            loadingTag += "</div>";
	            loadingTag += "<div class=\"LoadingBackground\" style=\"height:" + String(h) + "px\">";
	            loadingTag += "</div>";
	            loadingTag += "</div>";
	            $("body").prepend(loadingTag);

	            $(window).resize(function () {
	                if (document.getElementById("LoadingDivLayer") != null) {
	                    var dh = $(document).outerHeight(true);
	                    var wh = $(window).outerHeight(true);
	                    var h = (dh > wh) ? dh : wh;

	                    var dw = $(document).outerWidth(true);
	                    var ww = $(window).outerWidth(true);
	                    var w = (dw > ww) ? dw : ww;

	                    var posX = (w / 2) + 14;
	                    var posY = (h / 2) + 14;

	                    $("#LoadingDivLayer").css({
	                        height: String(h) + "px"
	                    });

	                    $("#LoadingDivLayer > .LoadingForeground").css({
	                        height: String(h) + "px"
	                    });

	                    $("#LoadingDivLayer > .LoadingForeground > .LoadingBox").css({
	                        top: String(posY) + "px",
	                        left: String(posX) + "px"
	                    });

	                    $("#LoadingDivLayer > .LoadingBackground").css({
	                        height: String(h) + "px"
	                    });
	                }
	            });
	        }
	    } else {
	        var off = $(this).offset();
	        var posX = ($(this).width() / 2) - 34;
	        var posY = ($(this).height() / 2) - 34;

	        var loadingTag = "<div id=\"LoadingDivLayer\" class=\"LoadingDivLayerClass\" style=\"width:" + String($(this).width()) + "px;height:" + String($(this).height()) + "px;top:" + String(off.top) + "px;left:" + String(off.left) + "px;\">";
	        loadingTag += "<div class=\"LoadingForeground\" style=\"height:" + String($(this).height()) + "px;\">";
	        loadingTag += "<div class=\"LoadingBox\" style=\"top:" + String(posY) + "px;left:" + String(posX) + "px;\">";
	        loadingTag += "<img src=\"" + LoadingImage + "\" alt=\"Loading...\" />";
	        loadingTag += "</div>";
	        loadingTag += "</div>";
	        loadingTag += "<div class=\"LoadingBackground\" style=\"width:" + String($(this).width()) + "px;height:" + String($(this).height()) + "px\">";
	        loadingTag += "</div>";
	        loadingTag += "</div>";
	        $(this).prepend(loadingTag);
	    }

		$("#LoadingDivLayer").fadeIn(350);
	} else {
		if (document.getElementById("LoadingDivLayer") != null) {
			$("#LoadingDivLayer").remove();
		}
	}
};
