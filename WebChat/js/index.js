var temp_chatlog="";
var page_n = 1;
var this_t;
var pageCount;
function clickOn(ws, username) {

    $("#user_list #myfriend").unbind("click").click(function () {
        $("#user_list a").removeClass("active");
        //$("#user_list #myfriend").addClass("list-group-item");
        $(this).addClass(" active");
        $("#chat_with").html($(this).children(".nickname").text());
        $("#msg_clear").show();
        $("#addMC").hide();
        $("#vDGmember").hide();
        var div_msgbox = document.getElementById("div_msgbox");
        div_msgbox.innerHTML = "";
        var arr = $(this).find(".badge");
        arr[0].innerHTML = "";


        editor.sync();
        editor.html("");
        $("#sendForm").show();
        $("#btn_say").show();
        var mge = { "action": "getChatlog", "username": username, "chatwith": $(this).children(".nickname").text()};
        var msg = JSON.stringify(mge);
        ws.send(msg);
    });

    $("#user_list #myDG").unbind("click").click(function () {
        $("#user_list a").removeClass("active");
        //$("#user_list #myDG").addClass("list-group-item");
        $(this).addClass(" active");
        $("#chat_with").html($(this).children(".nickname").text());
        $("#msg_clear").show();
        $("#addMC").hide();
        $("#vDGmember").show();


        editor.sync();
        editor.html("");
        $("#sendForm").show();
        $("#btn_say").show();
        var div_msgbox = document.getElementById("div_msgbox");
        div_msgbox.innerHTML = "";
        var arr = $(this).find(".badge");
        arr[0].innerHTML = "";

        var mge = { "action": "getChatlog", "username": username, "chatwith": $(this).children(".nickname").text() };
        var msg = JSON.stringify(mge);
        ws.send(msg);
    });

    $("#user_list #public_chat").unbind("click").click(function () {
        $("#user_list a").removeClass("active");
        //$("#user_list #myDG").addClass("list-group-item");
        $(this).addClass(" active");
        $("#chat_with").html($(this).children(".nickname").text());
        $("#msg_clear").show();
        $("#addMC").hide();
        $("#vDGmember").hide();

        editor.sync();
        editor.html("");
        $("#sendForm").show();
        $("#btn_say").show();
        var div_msgbox = document.getElementById("div_msgbox");
        div_msgbox.innerHTML = temp_chatlog;
        var arr = $(this).find(".badge");
        arr[0].innerHTML = "";
        div_msgbox.scrollTop = div_msgbox.scrollHeight;
    });

    $("#MCfriends").unbind("click").click(function () {
        $("#user_list a").removeClass("active");
        //$("#user_list #myDG").addClass("list-group-item");
        $(this).addClass(" active");
        $("#chat_with").html($(this).children(".nickname").text());
        $("#msg_clear").hide();
        $("#addMC").show();
        $("#vDGmember").hide();

        editor.sync();
        editor.html("");
        $("#sendForm").hide();
        $("#btn_say").hide();


        var head = "<header><img id='bg' src='../images/bg.jpg'><p id='user-name' class='data-name'>"+username+"</p><a href='#' id='avt' class='btn btn-info btn-lg data-avt'><span class='glyphicon glyphicon-user'></span></a></header><ul id='MYmc_mge'></ul>";
        var div_msgbox = document.getElementById("div_msgbox");
        div_msgbox.innerHTML = "";
        div_msgbox.innerHTML += head;

        $("#MyMCfalse").unbind("click").click(function () {
            $(".review").hide();
        });

        var mge = { "action": "getMYmc", "username": username, "page_n": "1" };
        page_n = 1;
        var msg = JSON.stringify(mge);
        ws.send(msg);
    });

    $("#MYmc_mge .comme").unbind("click").click(function () {
        this_t = this;
        var temp = $("#testc").find(".ke-container");
        temp[0].className += " test";
        $(".comment").show();
        $("#COMfalse").unbind("click").click(function () {
            $(".comment").hide();
        });
        var id = this.id;
        $("#PCOM").unbind("click").click(function () {
            publishComment(ws, username, id);
        });
    });

    $("#MYmc_mge .support").unbind("click").click(function () {
        this_t = this;
        if (this.childNodes[0].className == "glyphicon glyphicon-heart-empty")
            support(ws, username);
        else if (this.childNodes[0].className == "glyphicon glyphicon-heart")
            disSupport(ws, username);
    });
    return;
}//点击联系人


function getNowFormatDate() {
    var date = new Date();
    var seperator1 = "-";
    var seperator2 = ":";
    var month = date.getMonth() + 1;
    var strDate = date.getDate();
    if (month >= 1 && month <= 9) {
        month = "0" + month;
    }
    if (strDate >= 0 && strDate <= 9) {
        strDate = "0" + strDate;
    }
    var currentdate = date.getFullYear() + seperator1 + month + seperator1 + strDate
            + " " + date.getHours() + seperator2 + date.getMinutes();
    return currentdate;
}//获取时间

function getCookie(username) {
    var arr = document.cookie.match(new RegExp("(^| )" + username + "=([^;]*)(;|$)"));
    if (arr != null) {
        return unescape(arr[2]);
    } else {
        return null;
    }
}//获得cookie


function talkToSomeone(ws, username) {
    editor.sync();
    var isayp1 = "<div class='chatbox' style='float:right'><div style='text-align:right'><span style='color:#d2d2d2;'>";
    var isayp2 = "</span><span style='font-weight:900;color:#6d6d6d'>";
    var isayp3 = "</span></div><div class='chatarrow' style='right:5px;border-bottom: 8px solid #ffe6b8;'></div><div class='chat selfchat' style='max-width:504px;'>";
    var isayp4 = "<br></div></div><div class='clearboth'></div>";

    var MGpart1 = "<div class='chatbox' style='float:left'><div style='text-align:left'><span style='font-weight:900;color:#6d6d6d'>";
    var MGpart2 = "</span><span style='color:#d2d2d2;'>";
    var MGpart3 = "</span></div><div class='chatarrow' style='left:5px;border-bottom: 8px solid #cfffcf;'></div><div class='chat' style='max-width:769px;'>";
    var MGpart4 = "<br></div></div><div class='clearboth'></div>";

    var chat_with = document.getElementById("chat_with");
    var inp_say = document.getElementById("inp_say");
    if (inp_say.value == "") {
        alert("内容不能为空哟！");
        return;
    }
    var div_msgbox = document.getElementById("div_msgbox");

    var timeNow = getNowFormatDate();     //获取当前时间

    var context = MGpart1 + username + MGpart2 + timeNow + MGpart3 + inp_say.value + MGpart4;
    var chatlog = isayp1 + timeNow + isayp2 + username + isayp3 + inp_say.value + isayp4;

    var mge = { "action": "talk", "username": username, "chatwith": chat_with.innerText, "chatcontext": context, "chatlog": chatlog};
    var msg = JSON.stringify(mge);

    div_msgbox.innerHTML += isayp1 + timeNow + isayp2 + username + isayp3 + inp_say.value + isayp4;

    editor.html("");
    ws.send(msg);
    div_msgbox.scrollTop = div_msgbox.scrollHeight;
    return;
}//跟某好友聊天

function talkToDG(ws, username) {
    editor.sync();
    var isayp1 = "<div class='chatbox' style='float:right'><div style='text-align:right'><span style='color:#d2d2d2;'>";
    var isayp2 = "</span><span style='font-weight:900;color:#6d6d6d'>";
    var isayp3 = "</span></div><div class='chatarrow' style='right:5px;border-bottom: 8px solid #ffe6b8;'></div><div class='chat selfchat' style='max-width:504px;'>";
    var isayp4 = "<br></div></div><div class='clearboth'></div>";

    var MGpart1 = "<div class='chatbox' style='float:left'><div style='text-align:left'><span style='font-weight:900;color:#6d6d6d'>";
    var MGpart2 = "</span><span style='color:#d2d2d2;'>";
    var MGpart3 = "</span></div><div class='chatarrow' style='left:5px;border-bottom: 8px solid #cfffcf;'></div><div class='chat' style='max-width:769px;'>";
    var MGpart4 = "<br></div></div><div class='clearboth'></div>";

    var chat_with = document.getElementById("chat_with");
    var inp_say = document.getElementById("inp_say");
    if (inp_say.value == "") {
        alert("内容不能为空哟！");
        return;
    }
    var div_msgbox = document.getElementById("div_msgbox");

    var timeNow = getNowFormatDate();     //获取当前时间

    var context = MGpart1 + username + MGpart2 + timeNow + MGpart3 + inp_say.value + MGpart4;
    var chatlog = isayp1 + timeNow + isayp2 + username + isayp3 + inp_say.value + isayp4;

    var mge = { "action": "talkToDG", "username": username, "chatwith": chat_with.innerText, "chatcontext": context, "chatlog": chatlog };
    var msg = JSON.stringify(mge);

    div_msgbox.innerHTML += isayp1 + timeNow + isayp2 + username + isayp3 + inp_say.value + isayp4;

    editor.html("");
    ws.send(msg);
    div_msgbox.scrollTop = div_msgbox.scrollHeight;
    return;
}//讨论组！

function talkToAll(ws, username) {
    editor.sync();
    var isayp1 = "<div class='chatbox' style='float:right'><div style='text-align:right'><span style='color:#d2d2d2;'>";
    var isayp2 = "</span><span style='font-weight:900;color:#6d6d6d'>";
    var isayp3 = "</span></div><div class='chatarrow' style='right:5px;border-bottom: 8px solid #ffe6b8;'></div><div class='chat selfchat' style='max-width:504px;'>";

    var MGpart1 = "<div class='chatbox' style='float:left'><div style='text-align:left'><span style='font-weight:900;color:#6d6d6d'>";
    var MGpart2 = "</span><span style='color:#d2d2d2;'>";
    var MGpart3 = "</span></div><div class='chatarrow' style='left:5px;border-bottom: 8px solid #cfffcf;'></div><div class='chat' style='max-width:769px;'>";
    var MGpart4 = "<br></div></div><div class='clearboth'></div>";

    var isayp4 = "<br></div></div><div class='clearboth'></div>";
    var chat_with = document.getElementById("chat_with");
    var inp_say = document.getElementById("inp_say");
    if (inp_say.value == "") {
        alert("内容不能为空哟！");
        return;
    }
    var div_msgbox = document.getElementById("div_msgbox");

    var timeNow = getNowFormatDate();     //获取当前时间

    div_msgbox.innerHTML += isayp1 + timeNow + isayp2 + username + isayp3 + inp_say.value + isayp4;
    var context = MGpart1 + username + MGpart2 + timeNow + MGpart3 + inp_say.value + MGpart4;
    temp_chatlog += isayp1 + timeNow + isayp2 + username + isayp3 + inp_say.value + isayp4;
    var mge = { "action": "talkToAll", "username": username, "chatwith": chat_with.innerText, "chatcontext": context, };
    var msg = JSON.stringify(mge);

    editor.html("");
    ws.send(msg);
    div_msgbox.scrollTop = div_msgbox.scrollHeight;
    return;
}//公共聊天室


function beTold(mge) {
    var div_msgbox = document.getElementById("div_msgbox");

    var timeNow = getNowFormatDate();    //获取当前时间
    var temp = $("#user_list").find(".active");
    if (temp[0].innerText.indexOf(mge.friendname) >=0 ) {
        div_msgbox.innerHTML += mge.chatcontext;
        div_msgbox.scrollTop = div_msgbox.scrollHeight;
    }
    else {
        var arr = $("#user_list a").find(".nickname");
        var arr1 = $("#user_list a").find(".badge");
        for (var i = 0; i < arr.length; i++) {
            if (arr[i].innerText == mge.friendname) {
                arr1[i].innerHTML = "New";
            }
        }
    }
    return;
}//收到好友或者讨论组消息


function beToldToAll(mge) {
    var div_msgbox = document.getElementById("div_msgbox");

    var timeNow = getNowFormatDate();     //获取当前时间
    var chat_with = document.getElementById("chat_with");
    if (chat_with.innerHTML != "公共聊天室") {
        var temp = $("#public_chat").find(".badge");
        temp[0].innerHTML = "New";
        temp_chatlog += mge.chatcontext;
    }
    else {
        temp_chatlog += mge.chatcontext;
        div_msgbox.innerHTML += mge.chatcontext;
        div_msgbox.scrollTop = div_msgbox.scrollHeight;
    }
    return;
}//收到公共聊天室消息


function checkTalkToWho(ws,username) {
    var chat_with = document.getElementById("chat_with");
    var temp = $("#user_list").find(".active");
    if (temp[0].id == "myDG") {
        talkToDG(ws,username);
    }
    else if (temp[0].id == "myfriend") {
        talkToSomeone(ws,username);
    }
    else if (temp[0].id == "public_chat") {
        talkToAll(ws,username);
    }
    //if (chat_with.innerHTML == "公共聊天室") {
    //    talkToAll(ws, username);
    //}
    //else {
    //    talkToSomeone(ws, username);
    //}
    return;
}//检测跟谁聊天


function findFriend(ws,username) {
    var Fname = document.getElementById("strFName");
    if (Fname.value == "") {
        alert("内容不能为空哟！");
        return;
    }
    var mge = { "action": "findFriend", "username": username, "friendname": Fname.value };
    var msg = JSON.stringify(mge);
    ws.send(msg);
    return;
}//搜索好友


function getFriend(mgs) {
    var friendList = document.getElementById("friendList");
    friendList.innerHTML = "";
    var MFpart1 = "<a id='";
    var MFpart2 = "' class='list-group-item' onfocus='this.blur()' onclick=''><span class='badge'></span><span style='margin-right:5px;' class='glyphicon glyphicon-user'></span><span class='nickname'>";
    var Mfpart3 = "<span></span></span></a>";
    if (mgs.arrfriend.length == 1 && mgs.arrfriend[0] == "")
    {
        alert("查无此人！")
        return;
    }
    for (var i = 0; i < mgs.arrfriend.length; i++) {
        if (mgs.arrfriend[i] != null)
            friendList.innerHTML += MFpart1 + MFpart2 + mgs.arrfriend[i] + Mfpart3;
    }
    return;
}//获得服务端发来的搜索好友结果


function addFriend(ws,username) {
    $("#friendList a").click(function () {
        var temp = $(this).find(".nickname");
        if (confirm("确定添加" + temp[0].innerText + "好友？")) {
            var mge = { "action": "addFriend", "username": username, "friendname": temp[0].innerText};
            var msg = JSON.stringify(mge);
            ws.send(msg);
        }
    });
    return;
}//点击搜索到的对应好友添加


function checkAddFriend(mgs) {
    if (mgs.addfriend == 1) {
        alert("成功添加" + mgs.friendname + "为好友！");
    }
    else if (mgs.addfriend == 0) {
        alert("无法添加自己为好友！");
    }
    else if (mgs.addfriend == 2) {
        alert("已是好友关系！");
    }
    return;
}//判断添加好友成功与否

function checkCDG(mgs) {
    if(mgs.addfriend==1){
        alert("创建讨论组：【"+mgs.discussion_group_name+"】成功！");
    }
    else {
        alert("未知错误！");
    }
    var temp = document.getElementById("DiscussionGroupName");
    temp.value = "";
    temp = document.getElementById("allFriendList");
    temp.innerHTML = "";
    $(".pro_stop").hide();
}//检查创建讨论组是否成功

function createDiscussionGroup(ws, username) {
    var arr = $("#allFriendList").find(".active");
    var discussion_group_name = document.getElementById("DiscussionGroupName").value;
    if (discussion_group_name == "") {
        alert("讨论组名不能为空！");
        return;
    }
    var discussion_group_members = new Array();
    discussion_group_members[0] = username;
    for (var i = 0; i < arr.length; i++) {
        discussion_group_members[i+1] = arr[i].innerText;
    }
    var mge = { "action": "createDiscussionGroup", "username": username, "discussion_group_name": discussion_group_name, "discussion_group_members": discussion_group_members };
    var msg = JSON.stringify(mge);
    ws.send(msg);
}//创建讨论组

function showCreateDiscussionGroup(ws,username) {
    var allFriendList = document.getElementById("allFriendList");
    //var MFpart1 = "<a id='' class='list-group-item' onfocus='this.blur()' onclick=''><span class='badge'></span><span style='margin-right:5px;' class='glyphicon glyphicon-user'></span><span class='nickname'>";
    //var Mfpart3 = "<span></span></span></a>";
    //for (var i = 0; i < mgs.arrallmyfriend.length; i++) {
    //    allFriendList.innerHTML += MFpart1 + mgs.arrallmyfriend[i] + Mfpart3;
    //}
  
    var arr = $("#user_list").find("a#myfriend");
    for (var i = 0; i < arr.length; i++) {
        //arr[i].outerHTML.removeClass("active");
        arr[i].className = "list-group-item-un list-group-item";
        allFriendList.innerHTML += arr[i].outerHTML;
    }
    $("#allFriendList a").unbind("click").click(function () {
        $("#allFriendList a").addClass("list-group-item");
        if ($(this).is(".active")) {
            $(this).removeClass("active");
        }
        else {
            $(this).addClass(" active");
        }
    });
    $("#Dfalse").click(function () {
        var temp = document.getElementById("DiscussionGroupName");
        temp.value = "";
        temp = document.getElementById("allFriendList");
        temp.innerHTML = "";
        $(".pro_stop").hide();
    });
    $("#Create").unbind("click").click(function () {
        createDiscussionGroup(ws,username);
    });
}//显示创建讨论组面板

function showAddFriend(ws,username) {
    $(".pop_bg").show();
    $("#false").click(function () {
        $(".pop_bg").hide();
        var temp = document.getElementById("strFName");
        temp.value = "";
        temp = document.getElementById("friendList");
        temp.innerHTML = "";
    });
    $("#Search").click(function () {
        findFriend(ws, username);
    });
}//显示添加好友对话框


function showDGmember(ws, username) {
    $(".pop_bg").show();
    $("#addFD").html("谈论组成员");
    $("#FNAMe").hide();
    $("#false").click(function () {
        $(".pop_bg").hide();
        $("#addFD").html("添加好友");
        $("#FNAMe").show();
        var temp = document.getElementById("strFName");
        temp.value = "";
        temp = document.getElementById("friendList");
        temp.innerHTML = "";
    });
    var Fname = document.getElementById("chat_with");
    var mge = { "action": "getDGmember", "username": username, "friendname": Fname.innerHTML };
    var msg = JSON.stringify(mge);
    ws.send(msg);
}

function UpdateMFList(mge) {
    var MF_list = document.getElementById("user_list");
    MF_list.innerHTML = "<a id='public_chat' class='list-group-item' onfocus='this.blur()' onclick=''><span class='badge'></span><span style='margin-right:5px;' class='glyphicon glyphicon-th'></span><span class='nickname'>公共聊天室<span></span></span></a>";
    MF_list.innerHTML += "<a id='MCfriends' class='list-group-item' onfocus='this.blur()'><span class='badge'></span><span style='margin-right:5px;' class='glyphicon glyphicon-home'></span><span class='nickname'>我的朋友圈<span></span></span><span style='float:right;margin-right: 5px;color:red;' class='glyphicon glyphicon-fire'></span></a>";
    var nowusers_count = document.getElementById("nowusers_count");

    var MFpart1 = "<a id='myfriend";
    var MFpart2_1 = "' class='list-group-item-un' onfocus='this.blur()' onclick=''><span class='badge'></span><span style='margin-right:5px;' class='glyphicon glyphicon-user'></span><span class='nickname'>";
    var MFpart2 = "' class='list-group-item' onfocus='this.blur()' onclick=''><span class='badge'></span><span style='margin-right:5px;' class='glyphicon glyphicon-user'></span><span class='nickname'>";
    var Mfpart3 = "<span></span></span></a>";

    var MDpart1 = "<a id='myDG' class='list-group-item-DG' onfocus='this.blur()' onclick=''><span class='badge' style='margin-left:195px;position: absolute;'></span><span style='margin-right:5px;' class='glyphicon glyphicon-th-large'></span><span class='nickname'>";
    var MDpart2 = "<span></span></span></a>";

    var arr = $("#user_list a").find(".nickname");


    var unonl=0;
    for (var i = 0; i < mge.arrolfriend.length; i++) {
        if (mge.arrolfriend[i] == null) {
            unonl++;
            continue;
        }
        $("#MCfriends").after(MFpart1 + MFpart2 + mge.arrolfriend[i] + Mfpart3);
            //MF_list.innerHTML += MFpart1 + MFpart2 + mge[i] + Mfpart3;
    }
    var arr = $("#user_list a").find(".nickname");
    for (var i = 0; i < mge.arrallmyfriend.length; i++) {
        var t = 0;
        for (var j = 0; j < arr.length; j++) {
            if (arr[j].innerText != mge.arrallmyfriend[i]) {
                t++;
            }
        }
        if (t == arr.length) {
            MF_list.innerHTML += MFpart1 + MFpart2_1 + mge.arrallmyfriend[i] + Mfpart3;
        }
    }

    for (var i = 0; i < mge.mydiscussiongroups.length; i++) {
        $("#MCfriends").after(MDpart1 + mge.mydiscussiongroups[i] + MDpart2);
    }

    nowusers_count.innerHTML =String(mge.arrolfriend.length-unonl)+ "/" + String(mge.arrallmyfriend.length);
    arr = $("#user_list a").find(".nickname");
    var chat_with = document.getElementById("chat_with");
    for (var i = 0; i < arr.length; i++) {
        if (chat_with.innerText == arr[i].innerText) {
            arr[i].parentElement.className += " active";
        }
    }
}//刷新好友列表


function getChatLog(mge) {
    var div_msgbox = document.getElementById("div_msgbox");
    var chat_with = document.getElementById("chat_with");
    if (chat_with.innerText == mge.chatwith) {
        div_msgbox.innerHTML = "";
        div_msgbox.innerHTML = mge.chatlog;
        div_msgbox.scrollTop = div_msgbox.scrollHeight;
    }
}//获取聊天记录

function delChatLog(ws, username) {
    var chatwith = document.getElementById("chat_with").innerText;
    if (confirm("确定删除与【" + chatwith + "】的聊天记录？")) {
        var mge = { "action": "delChatlog", "username": username, "chatwith": chatwith };
        var msg = JSON.stringify(mge);
        ws.send(msg);
        var div_msgbox = document.getElementById("div_msgbox");
        div_msgbox.innerHTML = "";
    }
    return;
}//删除聊天记录

function publishMYmc(ws, username) {
    editor2.sync();
    var inp_MCfriends = document.getElementById("inp_MCfriends");
    if (inp_MCfriends.value == "") {
        alert("内容不能为空哟！");
        return;
    }
    var context = inp_MCfriends.value;
    var timeNow = getNowFormatDate();
    var mge = { "action": "publishMYmc", "username": username, "chatcontext": context,"publishtime":timeNow};
    var msg = JSON.stringify(mge);
    ws.send(msg);
    
    mge = { "action": "getMYmc", "username": username, "page_n": "1" };
    page_n = 1;
    msg = JSON.stringify(mge);
    ws.send(msg);
    editor2.html("");
    $(".review").hide();
    var MYmc_mge = document.getElementById("MYmc_mge");
    MYmc_mge.innerHTML = "";
}//发表朋友圈

function getMYmcs(mge) {
    if (mge.page_n == 1)
        pageCount = mge.mymcid;
    var temp;
    var MYmc_mge = document.getElementById("MYmc_mge");
    var part1 = "<li><div class='po-avt-wrap'><a href='#' class='btn btn-info btn-lg po-avt'><span class='glyphicon glyphicon-user'></span></a></div><div class='po-cmt'><div class='po-hd'><p class='po-name'><span class='data-name'>";
    var part2 = "</span></p><div class='post'>";
    var part3 = " </div><p class='time'>";
    var part4 = "</p><img class='c-icon' src='../images/c.png'></div>";
    var part7 = "</p><div class='c-icon'><a class='support' id='";
    var part6 = "'><span class='glyphicon glyphicon-heart-empty'></span><span id='Zan'>&nbsp;&nbsp;赞</span></a><span style='font-size:20px'>&nbsp;|&nbsp;</span><a class='comme' id='";
    var part6_2 = "'><span class='glyphicon glyphicon-heart'></span><span id='Zan'>取消</span></a><span style='font-size:20px'>&nbsp;|&nbsp;</span><a class='comme' id='";
    var part5 = "'></span><span class='glyphicon glyphicon-edit'></span>&nbsp;&nbsp;评论</a></div><div class='r'></div><div class='cmt-wrap'><div class='like'><img src='../images/l.png'><span>";

    var part8 = "</div>";

    var pl1 = "<p><span class='sup'>";
    var pl2 = "</span>";
    var pl3 = "</p>";
    for (var i = 0; i < mge.MYmcs.length; i++) {

        var c = 0;
        temp = part1 + mge.MYmcs[i].username + part2 + mge.MYmcs[i].context + part3 + mge.MYmcs[i].ptime + part7 + mge.MYmcs[i].id;
        if (mge.MYmcs[i].supports != null) {
            for (var j = 0; j < mge.MYmcs[i].supports.length; j++) {
                if (mge.MYmcs[i].supports[j] == mge.username)
                    c++;
            }
        }
        if (c == 0) {
            temp += part6;
        }
        else if (c > 0) {
            temp += part6_2;
        }
        temp += mge.MYmcs[i].id + part5;

       if (mge.MYmcs[i].supports != null) {
            for (var j = 0; j < mge.MYmcs[i].supports.length; j++) {
                temp += mge.MYmcs[i].supports[j];
                if (j != mge.MYmcs[i].supports.length - 1)
                    temp += ",";
            }
       }
       temp += "</span>"+part8;
       temp += "<div class='cmt-div_msgbox'>";



        if (mge.MYmcs[i].comments.length != 0) {
            if (mge.MYmcs[i].comments.length == 1)
                temp += pl1 + mge.MYmcs[i].comments[0].username+":" + pl2 + mge.MYmcs[i].comments[0].context + pl3;
            else {
                temp += pl1 + mge.MYmcs[i].comments[0].username + ":" + pl2 + mge.MYmcs[i].comments[0].context + pl3;
                for (var j = 1; j < mge.MYmcs[i].comments.length; j++) {
                    temp += "<p><span class='sup'>" + mge.MYmcs[i].comments[j].username + ":" + pl2 + mge.MYmcs[i].comments[j].context + pl3;
                }
            }
        }
        temp += part8;
        MYmc_mge.innerHTML += temp + part8;
    }
    return;
}//获取朋友圈

function publishComment(ws,username,id) {
    editor3.sync();
    var inp_comment = document.getElementById("inp_comment");
    if (inp_comment.value == "") {
        alert("内容不能为空哟！");
        return;
    }
    var context = inp_comment.value;
    var timeNow = getNowFormatDate();
    var mge = { "action": "publishComment","mymcid":id, "username": username, "chatcontext": context, "publishtime": timeNow };
    var msg = JSON.stringify(mge);
    ws.send(msg);
    editor3.sync();
    editor3.html("");
    $(".comment").hide();
    return;
}//评论

function publishCommentSuccess(mge) {
    this_t.parentElement.parentNode.childNodes[5].childNodes[1].innerHTML = "";  
    if (mge.MYmcs != null) {
        for (var j = 0; j < mge.MYmcs[0].comments.length; j++) {
            this_t.parentElement.parentNode.childNodes[5].childNodes[1].innerHTML += "<p><span class='sup'>" + mge.MYmcs[0].comments[j].username + ":" + "</span>" + mge.MYmcs[0].comments[j].context + "</p>";
        }
        return;
    }
}


function support(ws, username) {
        var mge = { "action": "support", "mymcid": this_t.id, "username": username };
        var msg = JSON.stringify(mge);
        ws.send(msg);
        return;
    }//赞

function disSupport(ws, username) {
        var mge = { "action": "disSupport", "mymcid": this_t.id, "username": username };
        var msg = JSON.stringify(mge);
        ws.send(msg);
        return;
    }//取消赞

function supportSuccess(mge) {
        this_t.parentElement.parentNode.childNodes[5].childNodes["0"].childNodes[1].innerHTML = "";
        this_t.childNodes[0].className = "glyphicon glyphicon-heart";
        this_t.childNodes[1].innerHTML = "取消";
        if (mge.arrallmyfriend != null) {
            for (var i = 0; i < mge.arrallmyfriend.length; i++) {
                this_t.parentElement.parentNode.childNodes[5].childNodes["0"].childNodes[1].innerHTML += mge.arrallmyfriend[i];
                if (i != mge.arrallmyfriend.length - 1)
                    this_t.parentElement.parentNode.childNodes[5].childNodes["0"].childNodes[1].innerHTML += ",";
            }
        }
        return;
    }//赞成

function disSupportSuccess(mge) {
        this_t.parentElement.parentNode.childNodes[5].childNodes["0"].childNodes[1].innerHTML = "";
        this_t.childNodes[0].className = "glyphicon glyphicon-heart-empty";
        this_t.childNodes[1].innerHTML = "&nbsp;&nbsp;赞";
        if (mge.arrallmyfriend != null) {
            for (var i = 0; i < mge.arrallmyfriend.length; i++) {
                this_t.parentElement.parentNode.childNodes[5].childNodes["0"].childNodes[1].innerHTML += mge.arrallmyfriend[i];
                if (i != mge.arrallmyfriend.length - 1)
                    this_t.parentElement.parentNode.childNodes[5].childNodes["0"].childNodes[1].innerHTML += ",";
            }
        }
        return;
    }//取消赞成功

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
    $("#addMC").hide();
    $("#vDGmember").hide();


    //初始化
    var chat_with = document.getElementById("chat_with");
    chat_with.innerHTML = "公共聊天室";
    $("#public_chat").addClass("list-group-item active");
    var div_msgbox = document.getElementById("div_msgbox");
    div_msgbox.innerHTML = "";




    mge = new Array;
    var massage;
    var username = getCookie("username");
    if (username == null || username == "") {
        location.href = "Login.html";
    }

    var myname = document.getElementById("myname");
    myname.value = username;




    var wsImpl = window.WebSocket || window.MozWebSocket;

    var sendForm = document.getElementById("sendForm");


    window.ws = new wsImpl('ws://118.89.50.76:8181');

    if (ws.readyState == 3) {
        alert("链接不上服务器！");
    }
    ws.onopen = function (evt) {
        var mge = { "action": "login", "username": username };
        var msg = JSON.stringify(mge);
        ws.send(msg);
    };
    ws.onmessage = function (evt) {
        var msg = JSON.parse(String(evt.data));
        switch (msg.action) {
            case "olfriend":
                UpdateMFList(msg);
                break;
            case "betold":
                beTold(msg);
                break;
            case "talkToAll":
                beToldToAll(msg);
                break;
            case "findFriend":
                getFriend(msg);
                addFriend(ws, username);
                break;
            case "addFriend":
                checkAddFriend(msg);
                break;
            case "getChatlog":
                getChatLog(msg);
                break;
            case "createDiscussionGroup":
                checkCDG(msg);
                break;
            case "getMYmc":
                getMYmcs(msg);
                break;
            case "disSupport":
                disSupportSuccess(msg);
                break;
            case "support":
                supportSuccess(msg);
                break;
            case "publishComment":
                publishCommentSuccess(msg);
                break;
        }
        clickOn(ws, username);
    }
    $("#btn_say").click(function () {
        checkTalkToWho(ws, username);
    });
    $("#addFriend").click(function () {
        showAddFriend(ws, username);
    });
    $("#vDGmember").click(function () {
        showDGmember(ws, username);
    });
    $("#addMC").click(function () {
        var temp = $("#testq").find(".ke-container");
        temp[0].className += " test";
        $(".review").show();
    });
    $("#createDiscussionGroup").click(function () {
        $(".pro_stop").show();
        showCreateDiscussionGroup(ws, username)
    });
    $("#PMyMC").click(function () {
        publishMYmc(ws, username);
    });

    $("#msg_clear").click(function () {
        delChatLog(ws, username);
    });

    $("#div_msgbox").scroll(function () {
        if((page_n-2)==pageCount)
            return;
        if (page_n == (pageCount + 1)) {
            $("<li><div style='text-align: center;'>无更多好友动态</div></li>").appendTo("#MYmc_mge");
            page_n++;
        }
        if (page_n == pageCount) {
            
            //var MYmc_mge = document.getElementById("MYmc_mge");
            //MYmc_mge.innerHTML += "<li><div style='text-align: center;'>无更多好友动态</div></li>";
            page_n++;
            return;
        }
        var chat_with = document.getElementById("chat_with");
        if (chat_with.innerHTML == "我的朋友圈") {
            var bot = 50;
            var div_msgbox = document.getElementById("div_msgbox");
            if ((bot + $("#div_msgbox").scrollTop()) >= (div_msgbox.scrollHeight - $("#div_msgbox").height())) {
                {
                    page_n++;
                    var mge = { "action": "getMYmc", "username": username, "page_n": page_n };
                    var msg = JSON.stringify(mge);
                    ws.send(msg);
                }
            }
        }
    });

});
