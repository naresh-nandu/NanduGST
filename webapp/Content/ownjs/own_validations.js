function textInput(event) {
    var value = String.fromCharCode(event.which);
    var pattern = new RegExp(/[a-zåäö ]/i);
    return pattern.test(value);
}

function numericInput(event) {
    var value = String.fromCharCode(event.which);
    var pattern = new RegExp(/[0-9]/i);
    return pattern.test(value);
}

