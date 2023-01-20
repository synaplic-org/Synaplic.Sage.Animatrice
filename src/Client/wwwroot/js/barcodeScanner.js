function InitBarcodeScanner(dotnetHelper) {

    Quagga.init({
        inputStream: {
            name: "Live",
            type: "LiveStream",
            target: document.querySelector('#barcodeScannerElement')
        },
        decoder: {
            // This is the type of barcode we are scanning. 
            // For barcodes other than books/ ISBNs, change this value.
            readers: ["ean_reader"]
        }
    }, function (err) {
        if (err) {
            console.log(err);
            return
        }
        console.log("Initialization finished. Ready to start");
        Quagga.start();
    });

    // When a barcode is detected...
    Quagga.onDetected(function (result) {

        // Obtain ISBN.
        var code = result.codeResult.code;

        // Pass to a .NET method.
        dotnetHelper.invokeMethodAsync("OnDetected", code);

        Quagga.offProcessed();
        Quagga.offDetected();
        Quagga.stop();
    });

}



var sScannedBarcode = "";
var sScannerDotnetHelper;

function addScannerEventListener(dotnetHelper) {

    if (sScannerDotnetHelper) {
        console.log("Removing keypress listener");
        // document.removeEventListener('keypress', scanKeypressHandler);
    }
    sScannerDotnetHelper = dotnetHelper;
    //console.log("Adding keypress listener");
    document.addEventListener('keypress', scanKeypressHandler);



}
var inputs = ['input', 'textarea'];
function scanKeypressHandler(e) {
    var activeElement = document.activeElement;
   

    if (activeElement && inputs.indexOf(activeElement.tagName.toLowerCase()) !== -1) {
        return;
    }

    if (e.keyCode == 13) {
        sScannerDotnetHelper.invokeMethodAsync("OnScanned", sScannedBarcode);
        //console.log("Scanned :" + sScannedBarcode);
        sScannedBarcode = "";
    } else {
        sScannedBarcode += e.key;
    }
}


//function InitBarcodeScannerEvent(dotnetHelper) {
//console.log("InitBarcodeScannerEvent  registring ...");
//    onScan.attachTo(document, {
//        reactToPaste: true, // Compatibility to built-in scanners in paste-mode (as opposed to keyboard-mode)
//        onScan: function (sCode, iQty) {
//            if (sCode != undefined) {
//                dotnetHelper.invokeMethodAsync("OnScanned", sCode);
//                console.log("Barcode scanner : " + sCode);
//            }
//        }
//    });

//}