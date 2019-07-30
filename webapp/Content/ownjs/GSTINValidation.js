function GSTINNo() {
    GST = document.getElementById("gst").value;
    PAN = document.getElementById("pan");
    SCODE = document.getElementById("statecode");
    if (GST != "") {
        var pat = /^[0-9]{2}[a-zA-Z]{5}[0-9]{4}[a-zA-Z]{1}[1-9a-zA-Z]{1}[zZ][0-9a-zA-Z]{1}$/;
        var res = pat.test(GST);
        if (res) {
            var statecode = GST.substr(0, 2);
            if (statecode == 00 || (statecode > 37 && statecode != 98)) {
                alert("State code should be within the range of 01 to 37");
                SCODE.value = "";
                PAN.value = GST.substr(2, 10);

            }
            else {
                SCODE.value = statecode;//.toString();
                PAN.value = GST.substr(2, 10);
            }
        }
        else {
            SCODE.value = "";
            PAN.value = "";
            alert("GSTIN is not in correct format.");

        }
    }
}