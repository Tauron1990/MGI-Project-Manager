"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function addObject(name, obj) {
    if (objects === null) {
        objects = {};
    }
    objects[name] = obj;
}
exports.addObject = addObject;
function actionFailure(args) {
    var temp;
    if (args.hasOwnProperty("error")) {
        temp = args.error[0].error;
    }
    else {
        temp = args[0].error;
    }
    formatError(temp);
}
exports.actionFailure = actionFailure;
function redictToHome() {
    window.setTimeout(function () {
        window.location.replace("/Index");
    }, 3000);
}
exports.redictToHome = redictToHome;
// ReSharper disable once InconsistentNaming
function SetupFormCheck(url) {
    if (url === void 0) { url = null; }
    var form = document.forms["SetupForm"];
    if (form != null) {
        form.submit();
        return;
    }
    if (url != null) {
        window.location.replace(url);
    }
}
exports.SetupFormCheck = SetupFormCheck;
// ReSharper disable once InconsistentNaming
function ToggleShowPass(passfield) {
    var ele = document.getElementById(passfield);
    if (ele != null) {
        if (ele.type === "text") {
            ele.type = "password";
        }
        else {
            ele.type = "text";
        }
    }
}
exports.ToggleShowPass = ToggleShowPass;
function getObject(name) {
    return objects[name];
}
function formatError(xhr) {
    var text = xhr.responseText;
    var contentspan = $("#ErrorListSpan");
    if (text.charAt(0) === "{") {
        var json = JSON.parse(text);
        var errors = json.errors;
        var ul = document.createElement("ul");
        for (var propertyName in errors) {
            if (errors.hasOwnProperty(propertyName)) {
                var il = document.createElement("li");
                il.className = "text-danger";
                il.appendChild(document.createTextNode(errors[propertyName]));
                ul.appendChild(il);
            }
        }
        contentspan.html(ul);
    }
    else {
        contentspan.html(text);
    }
    var dialogObj = getObject("ErrorList");
    dialogObj.show();
}
//# sourceMappingURL=Site.js.map