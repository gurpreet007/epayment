function GetDate() {
    return moment().format('D-MMM-YYYY');
//    var isChrome = /chrom/.test(navigator.userAgent.toLowerCase());
//    var d = new Date();
//    var day = d.getDate();
//    var month = d.getMonth() + 1;
//    var year = d.getFullYear();
//    day = (day < 10) ? "0" + day : day;
//    month = (month < 10) ? "0" + month : month;
//    if (isChrome) {
//        return (new Date().toJSON().substring(0, 10));
//    }
//    else {
//        return (day + '/' + month + '/' + year);
//    }
}