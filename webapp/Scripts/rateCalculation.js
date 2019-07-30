function taxableCalculation() {
    var qty = document.getElementById("qty").value;
    var unitprice = document.getElementById("unitprice").value;
    var discount = document.getElementById("discount").value;
    var rate = document.getElementById("rate").value;


    var igsta = document.getElementById("iamount");
    var cgsta = document.getElementById("camount");
    var sgsta = document.getElementById("samount");

    if ((qty != "") && (unitprice != "") && (discount != "")) {

        var discountValue;
        discountValue = (((qty * unitprice) / 100) * discount);
        document.getElementById("taxablevalue").value = (unitprice * qty) - discountValue;
    }
    var taxablevalue = document.getElementById("taxablevalue").value;

    if ((taxablevalue != "") && (rate != "")) {
        var cgstin = document.getElementById("cgstin").value;
        var gstin = document.getElementById("pos").value;
        var cgstin_state = cgstin.substring(0, 2);
        var gstin_state = gstin.substring(0, 2);
        if ((gstin_state != "") && (cgstin_state != "")) {
            if (gstin_state != cgstin_state) {
                igsta.value = (taxablevalue * rate) / 100;
                cgsta.value = "0";
                sgsta.value = "0";
            }
            else if (gstin_state == cgstin_state) {
                igsta.value = "0";
                cgsta.value = (taxablevalue * (rate / 2)) / 100;
                sgsta.value = (taxablevalue * (rate / 2)) / 100;
            }
        }
    }
}