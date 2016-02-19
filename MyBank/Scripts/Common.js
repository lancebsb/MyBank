// 时间格式
function DateFormatter(value, rowData, rowIndex) {
    if (value == undefined) {
        return "";
    }
    var dd = new Date();
    dd.getFullYear();
    /*json格式时间转js时间格式*/
    var arrDate = value.split('T');
    var newDate = arrDate[0];
    if (new Date(newDate, 'yyyy-MM-dd').getFullYear() < 1900) {
        return "";
    }
    return newDate;
}
function TimeFormatter(value, rowData, rowIndex) {
    if (value == undefined) {
        return "";
    }
    var dd = new Date();
    dd.getFullYear();
    /*json格式时间转js时间格式*/
    var arrDate = value.split('T');
    var newDate = arrDate[0];
    if (new Date(newDate, 'yyyy-MM-dd').getFullYear() < 1900) {
        return "";
    }
    return newDate+" "+arrDate[1].substr(0,8);
}

    //EasyUI用DataGrid用日期格式化
//function TimeFormatter(value, rec, index) {
//        if (value == undefined) {
//            return "";
//        }
//        /*json格式时间转js时间格式*/
//        value = value.substr(1, value.length - 2);
//        var obj = eval('(' + "{Date: new " + value + "}" + ')');
//        var dateValue = obj["Date"];
//        if (dateValue.getFullYear() < 1900) {
//            return "";
//        }
//        var val = dateValue.format("yyyy-mm-dd HH:MM");//控制格式
//        return val.substr(11, 5);
//    }

// 通用弹出框
function CommonSelect(url, title, width, height, divid) {
    var src;
    if (url.indexOf("?") != -1) {
        src = url + "&rnd=" + new Date().getTime();
    }
    else {
        src = url + "?rnd=" + new Date().getTime();
    }
    //var div = document.getElementById(divid);
    var div = $("#" + divid);
    div.html("");
    var iframe = document.createElement("iframe");
    iframe.id = "if1"
    iframe.src = "";
    iframe.width = "100%";
    iframe.height = "100%";
    iframe.border = "0";
    iframe.scrolling = "yes";
    div.append(iframe);

    $("#if1").attr("src", src);
    $("#if1").attr("frameborder", "0");
    $("#window").window({
        model: true,
        draggable: false,
        title: title,
        width: width,
        height: height,
        padding: 10,
        top: ($(window).height() - height) * 0.5,
        left: ($(window).width() - width) * 0.5
    }).window("open");
}
//重置Panel大小
function windowResize(col) {
    var width = $(window).width();
    var height = $(window).height();
    $("#" + col).width(width);
    //$('#' + col).height(100);
    $('#' + col).panel();
}
// 窗口关闭按钮
function Close() {
    window.parent.closeDialog();
}
// 窗口关闭按钮
function CloseAndFocus(id) {

    window.parent.closeDialog(id);
}
// 关闭前清空弹出窗口所有内容
function Destroy() {
    if ($("#form1").length > 0)
        $("#form1")[0].innerHTML = "";
}
//关闭Tab返回制定的Tab
function closeTab(title) {

    var tab = window.parent.$('#Tabs').tabs('getSelected');

    if (tab) {
        var finaTab = window.parent.$('#Tabs').tabs('getTab', title)
        if (finaTab) {
            window.parent.$('#Tabs').tabs('select', title);
        }
        window.parent.$('#Tabs').tabs('close', tab.panel('options').title);
    }
}




//弹出警告信息
showWarning = function (title, mes, lock, autoClose) {
    var icon = "warning";
    if (autoClose) {//自动关闭        
        showAlertAutodialog(title, mes, icon);
    } else {
        showAlertDialog(title, mes, lock, icon);
    }
}
//弹出错误信息
showError = function (title, mes, lock, autoClose) {
    var icon = "error";   
    if (autoClose) {//自动关闭        
        showAlertAutodialog(title, mes, icon);
    } else {
        showAlertDialog(title, mes, lock, icon);
    }
}
//弹出成功信息
showSucceed = function (title, mes,lock, autoClose) {
    var icon = "succeed";
    if (autoClose) {//自动关闭        
        showAlertAutodialog(title,mes,icon);
    } else {
        showAlertDialog(title, mes, lock, icon);
    }
}
showAlertDialog = function (title, mes, lock, icon) {
    var html = '<table><tr><td><img src="/Content/img/icons/' + icon + '.png" /></td><td>' + mes + '</td></tr></table>';
    var d = dialog({
        title: title,
        padding: 10,
        content: html,
        ok: true
    });
    if (lock)
        d.showModal();
    else
        d.show();
}
showAlertAutodialog = function (title, mes, icon) {
    var html = '<table><tr><td><img src="/Content/img/icons/' + icon + '.png" /></td><td>' + mes + '</td></tr></table>';
    var d = dialog({
        content: html,
        padding: 10
    });
    d.show();
    setTimeout(function () {
        d.close().remove();
    }, 2000);
}
//弹出询问信息
showQuestion = function (title, mes, lock, okFunc,cancelFunc) {
    var icon = "question"
    var html = '<table><tr><td><img src="/Content/img/icons/' + icon + '.png" /></td><td>' + mes + '</td></tr></table>';
    var d = dialog({
        title: title,
        padding: 20,
        content: html,
        ok: okFunc,
        cancel: cancelFunc
    });
    if (lock)
        d.showModal();
    else
        d.show();
}



