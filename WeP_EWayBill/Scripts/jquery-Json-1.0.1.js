var ReturnType = {
    Json: "Json",
    JsonObject : "JsonObject"
};

var JsonObject = function () {
    this.result = {};
    this.Add = function (key, val) {
        this.result[key] = val;
    };
    this.Get = function () {
        return this.result;
    };
    this.Set = function (key, value) {
        this.result[key] = value;
    };
    this.FindCheckboxValue = function (formID, targetName) {
        var arr = new Array();
        $("#" + formID).find("input:checkbox").each(function () {
            if ($(this).attr("name") == targetName) {
                if ($(this).is(":checked")) {
                    arr.push($(this).val());
                }
            }
        });

        var values = "";
        if (arr.length > 0) {
            for (var i = 0; i < arr.length; i++) {
                if (values == "") {
                    values = arr[i];
                } else {
                    values += "," + arr[i];
                }
            }
        }

        return { name: targetName, value: values };
    };
    this.FindRadioValue = function (formID, targetName) {
        var result = "";
        $("#" + formID).find("input:radio").each(function () {
            if ($(this).attr("name") == targetName) {
                if ($(this).is(":checked")) {
                    result = $(this).val();
                }
            }
        });

        return { name: targetName, value: result };
    };
    this.DynamicSubmit = function (submitURL, targetFrm, callback) {
        var tags = "<form name=\"DynamicJsonFormData\" id=\"DynamicJsonFormData\" method=\"post\" action=\"" + submitURL + "\" ";
        if (targetFrm != null) {
            tags += " target=\"" + targetFrm + "\" ";
        }
        tags += ">";

        for (var item in this.result) {
            tags += "<input type=\"hidden\" name=\"" + item + "\" value=\"" + this.result[item] + "\" />";
        }

        tags += "</form>";
        $("body").append(tags);
        setTimeout(function () {
            $("#DynamicJsonFormData").submit();
            $("#DynamicJsonFormData").remove();
            if (callback != null) {
                callback();
            }
        }, 100);
    }
};

$.fn.toJson = function (returnType) {
    var json = new JsonObject();
    var formID = this.attr("id");
    var uid = "";
    var Value = "";
    var name = "";

    $(this).find("input[type=text], input[type=number], input[type=password], input[type=tel], input[type=email], input[type=hidden], select, textarea").each(function () {
        uid = ($(this).attr("name") == null || $(this).attr("name") == "") ? $(this).attr("id") : $(this).attr("name");
        Value = $(this).val();

        if (uid != null && uid != "") {
            try {
                json.Add(uid, Value);
            } catch (e) {
                json.Add(uid, "");
            }
        }
    });

    $(this).find("input:radio").each(function () {
        if (name != $(this).attr("name")) {
            name = $(this).attr("name");
            if (name != null && name != "") {
                var radioData = json.FindRadioValue(formID, name);
                json.Add(radioData.name, radioData.value);
            }
        }
    });

    name = "";

    $(this).find("input:checkbox").each(function () {
        if (name != $(this).attr("name")) {
            name = $(this).attr("name");
            if (name != null && name != "") {
                var checkboxData = json.FindCheckboxValue(formID, name);
                json.Add(checkboxData.name, checkboxData.value);
            }
        }
    });

    switch (returnType) {
        case null:
        case ReturnType.Json:
            return json.Get();
            break;
        case ReturnType.JsonObject:
            return json;
            break;
        default:
            return json.Get();
            break;
    }
};