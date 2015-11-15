(function () {

    var jQueryElements = {
        addVerificationButton: $("#addVerification"),
        addParsedItemButton: $("#addParsedItem"),
        addHeaderItemButton: $("#addHeaderItem"),
        addVariableItemButton: $("#addVariableItem"),
    };

    var elements = {
        removeRow: "#removeRow",
        formGroup: ".form-group"
    };

    function appendDataItem(panelBody, data, elementPrefix) {

        var currentRow = panelBody.find(elements.formGroup + ":last-child");

        var rowNumber = 0;

        if (currentRow.length !== 0) {

            var firstInputName = currentRow.find("input:first").attr("name");
            
            //get the last index number of the row and increment it by 1
            rowNumber = parseInt(firstInputName.match(/\d/g)) + 1;
        }

        var newData = data.replace(/name="/g, 'name="' + elementPrefix + '[' + rowNumber + '].');
        panelBody.append(newData);
    }

    function setupButtons() {

        jQueryElements.addVerificationButton.click(function (e) {
            e.preventDefault();

            $.get("/TestCase/AddVerification", function (data) {
                appendDataItem(jQueryElements.addVerificationButton.parent().next(), data, "Verifications");
            });
        });

        jQueryElements.addParsedItemButton.click(function (e) {
            e.preventDefault();

            $.get("/TestCase/AddParseResponseItem", function (data) {
                appendDataItem(jQueryElements.addParsedItemButton.parent().next(), data, "ParseResponses");
            });
        });

        jQueryElements.addHeaderItemButton.click(function (e) {
            e.preventDefault();

            $.get("/TestCase/AddHeaderItem", function (data) {
                appendDataItem(jQueryElements.addHeaderItemButton.parent().next(), data, "Headers");
            });
        });

        jQueryElements.addVariableItemButton.click(function (e) {
            e.preventDefault();

            $.get("/TestFile/AddVariableItem", function (data) {
                appendDataItem(jQueryElements.addVariableItemButton.parent().next(), data, "Variables");
            });
        });

        $("body").on("click", elements.removeRow, function (e) {
            e.preventDefault();
            $(this).closest(elements.formGroup).remove();
        });
    }

    $(document).ready(function () {
        setupButtons();
    });

} ());