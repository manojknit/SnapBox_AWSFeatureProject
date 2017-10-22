
function validatefile() {
    var filePath = document.getElementById("file").value;
    var text = document.getElementById("fileName").value;
    var filename = filePath.replace(/^.*[\\\/]/, '');
    if (filename == '') {
        alert("Please select file.");
    }
    else if (filename == text) {
        //Match
        return true;
    }
    else {
        alert("Replaced file should have same name.");
    }
    return false;//false will not submit
}
