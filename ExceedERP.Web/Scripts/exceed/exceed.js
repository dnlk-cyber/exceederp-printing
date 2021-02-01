function appendZeros(num) {
    var length = 7;
    var numStr = num.toString();
    while (numStr.length < length) {
        numStr = '0' + numStr;
    }

    return numStr;
}