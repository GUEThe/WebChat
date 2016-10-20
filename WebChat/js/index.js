function getCookie(username) {
    var arr = document.cookie.match(new RegExp("(^| )" + username + "=([^;]*)(;|$)"));
    if (arr != null) {
        return unescape(arr[2]);
    } else {
        return null;
    }
}
$(document).ready(function () {


    $("#zone_left").height(window.innerHeight);
    $("#zone_left .list-group").height(window.innerHeight - 225);
    $("#div_msgpanel").width(.45 * (window.innerWidth - 100));
    $("#div_msgpanel").height(window.innerHeight - 200);
    $("#div_msgbox").height(window.innerHeight - 274);
    $("#div_privmsg").height(window.innerHeight - 274);
    $("#nav_action").width(window.innerWidth - $("#div_msgpanel").width() - $("#zone_left").width() - 40);
    $("#zone_left").show();
    $("#zone_center").show();//页面设置




    $("#user_list a").click(function () {
        $("#user_list a").removeClass("list-group-item active");
        $("#user_list a").addClass("list-group-item");
        $(this).addClass("active");
        $("#chat_with").html($(this).children(".nickname").text());
    });
    

    var div_msgbox = document.getElementById("div_msgbox");
    var isayp1="<div class='chatbox' style='float:right'><div style='text-align:right'><span style='color:#d2d2d2;'>";
    var isayp2="</span><span style='font-weight:900;color:#6d6d6d'>";
    var isayp3="</span></div><div class='chatarrow' style='right:5px;border-bottom: 8px solid #ffe6b8;'></div><div id='7f721f0620_05_33' class='chat selfchat' style='max-width:504px;'>";
    var isayp4 = "<br></div></div><div class='clearboth'></div>";






    var massage;
    var username = getCookie("username");
    
    var user_list = document.getElementById("user_list");
    var nowusers_count = document.getElementById("nowusers_count");

    //var inp_say = document.getElementById("inp_say");
    //var btn_say = document.getElementById("btn_say");

    var wsImpl = window.WebSocket || window.MozWebSocket;
    var sendForm = document.getElementById("sendForm");
    window.ws = new wsImpl('ws://125.217.34.53:8181');
    ws.onopen = function (evt) {
        massage = "login|" + username;
        ws.send(massage)
    };
    $("#btn_say").click(function () {
        var chat_with = document.getElementById("chat_with");
        var inp_say = document.getElementById("inp_say");
        massage = "talk|" + username + "|" + chat_with.innerText + "|" + inp_say.value;
        div_msgbox.innerHTML += isayp1 + "20;20;20" + isayp2 + username + isayp3 + inp_say.value + isayp4;
        inp_say.value = null;
        ws.send(massage);
    });
});
//var start = function () {
//    var inc = document.getElementById('incomming');
//    var wsImpl = window.WebSocket || window.MozWebSocket;
//    var form = document.getElementById('sendForm');
//    var input = document.getElementById('sendText');

//    inc.innerHTML += "connecting to server ..<br/>";

//    // create a new websocket and connectws://0.0.0.0:8181
//    window.ws = new wsImpl('ws://125.217.34.53:8181');

//    // when data is comming from the server, this metod is called
//    ws.onmessage = function (evt) {
//        inc.innerHTML += evt.data + '<br/>';
//    };

//    // when the connection is established, this method is called
//    ws.onopen = function (evt) {
//        inc.innerHTML += '.. connection open<br/>';
//        inc.innerHTML += evt.data + '<br/>';
//    };

//    // when the connection is closed, this method is called
//    ws.onclose = function () {
//        inc.innerHTML += '.. connection closed<br/>';
//    }

//    form.addEventListener('submit', function (e) {
//        e.preventDefault();
//        var val = input.value;
//        ws.send(val);
//        input.value = "";
//    });

//}
//window.onload = start

