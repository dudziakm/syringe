(function () {

    var jQueryElements = {
        addVerificationButton: $("#addVerification"),
        verificationDescription: $("#verificationDescription"),
        verificationRegex: $("#verificationRegex"),
        verificationType: $("#verificationType"),
        addParsedItemButton: $("#addParsedItem"),
        parseDescription: $("#parseDescription"),
        parseRegex: $("#parseRegex"),
        addHeaderItemButton: $("#addHeaderItem"),
        headerKey: $("#headerKey"),
        headerValue: $("#headerValue")
    };

    var elements = {
        removeRow: "#removeRow",
        formGroup: ".form-group"
    };

    function setupButtons() {

        jQueryElements.addVerificationButton.click(function (e) {
            e.preventDefault();

            var verificationItem = {
                Description: jQueryElements.verificationDescription.val(),
                Regex: jQueryElements.verificationRegex.val(),
                VerifyType: jQueryElements.verificationType.val()
            };
            $.get("/TestCase/AddVerification", verificationItem, function (data) {
                appendDataItem(jQueryElements.addVerificationButton, data, "Verifications");
                jQueryElements.verificationDescription.val('');
                jQueryElements.verificationRegex.val('');
            });
        });

        jQueryElements.addParsedItemButton.click(function (e) {
            e.preventDefault();

            var parseResponseItem = {
                Description: jQueryElements.parseDescription.val(),
                Regex: jQueryElements.parseRegex.val(),
            };
            $.get("/TestCase/AddParseResponseItem", parseResponseItem, function (data) {
                appendDataItem(jQueryElements.addParsedItemButton, data, "ParseResponses");
                jQueryElements.parseDescription.val('');
                jQueryElements.parseRegex.val('');
            });
        });

        jQueryElements.addHeaderItemButton.click(function (e) {
            e.preventDefault();

            var headerItem = {
                Key: jQueryElements.headerKey.val(),
                Value: jQueryElements.headerValue.val(),
            };
            $.get("/TestCase/AddHeaderItem", headerItem, function (data) {
                appendDataItem(jQueryElements.addHeaderItemButton, data, "Headers");
                jQueryElements.headerKey.val('');
                jQueryElements.headerValue.val('');
            });
        });

        $("body").on("click", elements.removeRow, function (e) {
            e.preventDefault();
            $(this).closest(elements.formGroup).remove();
        });
    }

    function appendDataItem(element, data, elementPrefix) {

        var currentRow = element.closest(elements.formGroup);
        var rowNumber = 0;
        var previousRow = currentRow.prev();

        //check if previous row exists then increase number
        if (previousRow.hasClass("form-group")) {
            var firstInputName = previousRow.find("input:first").attr("name");
            
            //get the last index number of the row and increment it by 1
            rowNumber = parseInt(firstInputName.match(/\d/g)) + 1;
        }

        var newData = data.replace(/name="/g, 'name="' + elementPrefix + '[' + rowNumber + '].');
        currentRow.before(newData);

    }

    $(document).ready(function () {
        setupButtons();
    });

} ());