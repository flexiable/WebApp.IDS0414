/// <reference path="oidc-client.js" />

function log() {
    //document.getElementById('results').innerText = '';

    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
        console.log( msg + '\r\n');
    });
}
window.onload = function () {
  
    if (!!document.getElementById("loginExternal")) {

        document.getElementById("loginExternal").addEventListener("click", login, false);
    }
    if (!!document.getElementById("logoutdiv")) {

        document.getElementById("logoutdiv").addEventListener("click", logout, false);
    }
   
}

var config = {
    authority: "https://localhost:5001",
    client_id: "js1",
    redirect_uri: "http://192.168.0.136:1919/Home/Callback",
    response_type: "code",
    scope:"openid profile api1",
    post_logout_redirect_uri: "http://192.168.0.136:1919/Home/index",
};
var mgr = new Oidc.UserManager(config);

mgr.getUser().then(function (user) {
    if (user) {
        log("User logged in", user.profile);
    }
    else {
        log("User not logged in");
    }
});

function login() {
    mgr.signinRedirect();
}

function api() {
    mgr.getUser().then(function (user) {
        var url = "https://localhost:6001/identity";

        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onload = function () {
            log(xhr.status, JSON.parse(xhr.responseText));
        }
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    });
}

function logout() {
    mgr.signoutRedirect();
}