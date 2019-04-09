"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function redictToHome() {
    window.setTimeout(() => {
        window.location.replace("/Index");
    }, 3000);
}
exports.redictToHome = redictToHome;
function toggleShowPass(passfield) {
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
exports.toggleShowPass = toggleShowPass;
//# sourceMappingURL=identityMain.js.map