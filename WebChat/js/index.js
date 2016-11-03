function clickOn() {
    $("#user_list a").click(function () {
        $("#user_list a").removeClass("list-group-item active");
        $("#user_list a").addClass("list-group-item");
        $(this).addClass("active");
        $("#chat_with").html($(this).children(".nickname").text());
    });
}


function getCookie(username) {
    var arr = document.cookie.match(new RegExp("(^| )" + username + "=([^;]*)(;|$)"));
    if (arr != null) {
        return unescape(arr[2]);
    } else {
        return null;
    }
}


function talkToSomeone(ws, username) {
    var isayp1 = "<div class='chatbox' style='float:right'><div style='text-align:right'><span style='color:#d2d2d2;'>";
    var isayp2 = "</span><span style='font-weight:900;color:#6d6d6d'>";
    var isayp3 = "</span></div><div class='chatarrow' style='right:5px;border-bottom: 8px solid #ffe6b8;'></div><div class='chat selfchat' style='max-width:504px;'>";
    var isayp4 = "<br></div></div><div class='clearboth'></div>";
    var chat_with = document.getElementById("chat_with");
    var inp_say = document.getElementById("inp_say");
    var div_msgbox = document.getElementById("div_msgbox");
    var myDate = new Date();
    var timeNow = myDate.toLocaleTimeString();     //获取当前时间
    if (String(inp_say.value).indexOf("|") >= 0) {
        alert("含有非法字符“|”！\n请重新输入！");
        return false;
    }
    if (chat_with.innerHTML == "公共聊天室") {
        massage = "talkToAll|" + username + "|" + chat_with.innerText + "|" + inp_say.value;
    }
    else {
        massage = "talk|" + username + "|" + chat_with.innerText + "|" + inp_say.value;
    }
    div_msgbox.innerHTML += isayp1 + timeNow + isayp2 + username + isayp3 + inp_say.value + isayp4;
    inp_say.value = null;
    ws.send(massage);
    div_msgbox.scrollTop = div_msgbox.scrollHeight;
}

function findFriend(ws,username) {
    var Fname = document.getElementById("strFName");
    var mgs = "findFriend|" + username + "|" + Fname.value;
    ws.send(mgs);
}
function getFriend(mgs) {
    var friendList = document.getElementById("friendList");
    var MFpart1 = "<a id='";
    var MFpart2 = "' class='list-group-item' onfocus='this.blur()' onclick=''><span class='badge'></span><span style='margin-right:5px;' class='glyphicon glyphicon-user'></span><span class='nickname'>";
    var Mfpart3 = "<span></span></span></a>";
    friendList.innerHTML = null;
    for (var i = 2; i < mgs.length; i++) {
        friendList.innerHTML += MFpart1 + MFpart2 + mgs[i] + Mfpart3;
    }

}
function addFriend(ws,username) {
    $("#friendList a").click(function () {
        var temp = $(this).find(".nickname");
        if (confirm("确定添加" + temp[0].innerText + "好友？")) {
            var mgs = "addFriend|" + username + "|" + temp[0].innerText;
            ws.send(mgs);
        }
    });
}
function chackAddFriend(mgs) {
    if (mgs[3] == 1) {
        alert("成功添加" + mgs[2] + "为好友！");
    }
    else if (mgs[3] == 0) {
        alert("无法添加自己为好友！");
    }
    else if (mgs[3] == 2) {
        alert("已是好友关系！");
    }
}
function beTold(mge) {
    var MGpart1="<div class='chatbox' style='float:left'><div style='text-align:left'><span style='font-weight:900;color:#6d6d6d'>";
    var MGpart2="</span><span style='color:#d2d2d2;'>";
    var MGpart3="</span></div><div class='chatarrow' style='left:5px;border-bottom: 8px solid #cfffcf;'></div><div class='chat' style='max-width:769px;'>";
    var MGpart4 = "<br></div></div><div class='clearboth'></div>";
    var div_msgbox = document.getElementById("div_msgbox");
    var myDate = new Date();
    var timeNow = myDate.toLocaleTimeString();     //获取当前时间
    //var id = escape(mge[1]);
    //id = id.replace(/\%u/g, "U");
    //$('#'+id).prepend("<span class='badge'>1</span>");
    $("#user_list a").removeClass("list-group-item active");
    $("#user_list a").addClass("list-group-item");
    var arr = $("#user_list a").find(".nickname");
    for (var i = 0; i < arr.length; i++) {
        if (arr[i].innerText == mge[1]) {
            arr[i].parentElement.className += " active";
        }        
    }
    var chat_with = document.getElementById("chat_with");
    if (chat_with.innerHTML != mge[1]) {
        chat_with.innerHTML = mge[1];
        div_msgbox.innerHTML = "";
    }
    div_msgbox.innerHTML += MGpart1 + mge[1] + MGpart2 + timeNow + MGpart3 + mge[2] + MGpart4;
    div_msgbox.scrollTop = div_msgbox.scrollHeight;
}
function UpdateMFList(mge) {
    var MF_list = document.getElementById("user_list");
    MF_list.innerHTML = "<a id='public_chat' class='list-group-item' onfocus='this.blur()'><span class='badge'></span><span style='margin-right:5px;' class='glyphicon glyphicon-user'></span><span class='nickname'>公共聊天室<span></span></span></a>";
    var nowusers_count = document.getElementById("nowusers_count");
    var MFpart1="<a id='";
    var MFpart2="' class='list-group-item' onfocus='this.blur()' onclick=''><span class='badge'></span><span style='margin-right:5px;' class='glyphicon glyphicon-user'></span><span class='nickname'>";
    var Mfpart3 = "<span></span></span></a>";
    var i = 1;
    for (i = 1; i < mge.length; i++) {
        var id = escape(mge[i]);
        id = id.replace(/\%u/g, "U");
        MF_list.innerHTML += MFpart1+MFpart2 + mge[i] + Mfpart3;
    }
    nowusers_count.innerHTML = mge.length - 1;
    var arr = $("#user_list a").find(".nickname");
    var chat_with = document.getElementById("chat_with");
    for (var i = 0; i < arr.length; i++) {
        if (chat_with.innerText == arr[i].innerText) {
            arr[i].parentElement.className += " active";
        }
    }
    //$("#public_chat").addClass("list-group-item active");
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

    //初始化
    var chat_with = document.getElementById("chat_with");
    chat_with.innerHTML = "公共聊天室";
    $("#public_chat").addClass("list-group-item active");
    var div_msgbox = document.getElementById("div_msgbox");
    div_msgbox.innerHTML = "";
    


    //$("#user_list a").click(function () {
    //    $("#user_list a").removeClass("list-group-item active");
    //    $("#user_list a").addClass("list-group-item");
    //    $(this).addClass("active");
    //    //var t = $(this).find(".nickname");
    //    //alert(t.innerText);
    //    $("#chat_with").html($(this).children(".nickname").text());
    //});
    clickOn();
    mge = new Array;
    var massage;
    var username = getCookie("username");
    //if (username == null || username == "")
    //{
    //    location.href = "Login.html";
    //}

    var myname = document.getElementById("myname");
    myname.value = username;

    //var inp_say = document.getElementById("inp_say");
    //var btn_say = document.getElementById("btn_say");

    var wsImpl = window.WebSocket || window.MozWebSocket;
    var sendForm = document.getElementById("sendForm");
    window.ws = new wsImpl('ws://192.168.3.12:8181');
    ws.onopen = function (evt) {
        massage = "login|" + username;
        ws.send(massage)
    };
    ws.onmessage = function (evt) {
        massage =String(evt.data);
        mge = massage.split("|");
        switch (mge[0]){
            case "olfriend":
                UpdateMFList(mge);
                clickOn();
                break;
            case "betold":
                beTold(mge);
                clickOn();
                break;
            case "findFriend":
                getFriend(mge);
                addFriend(ws, username);
                clickOn();
                break;
            case "addFriend":
                chackAddFriend(mge);
                clickOn();
                break;

        }
    }
    $("#btn_say").click(function () {
        talkToSomeone(ws,username);
    });
    $("#Search").click(function () {
        findFriend(ws,username);
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

