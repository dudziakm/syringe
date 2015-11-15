(function () {

    var jQueryElements = {
        addVerificationButton: $("#addVerification"),
        addParsedItemButton: $("#addParsedItem"),
        addHeaderItemButton: $("#addHeaderItem"),
        addVariableItemButton: $("#addVariableItem")
    };

    function appendDataItem(panelBody, html, elementPrefix) {

        var formGroup = panelBody.find(".form-group:last-child"), rowNumber = 0;

        if (formGroup.length !== 0) {
            var firstInputName = formGroup.find("input:first").attr("name");
            
            // get the last index number of the row and increment it by 1
            rowNumber = parseInt(firstInputName.match(/\d/g)) + 1;
        }

        // replace the name value with the correct prefix and row number so it can be posted to the server 
        var newHtml = html.replace(/name="/g, "name=\"" + elementPrefix + "[" + rowNumber + "].");
        panelBody.append(newHtml);
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

        $("body").on("click", "#removeRow", function (e) {
            e.preventDefault();
            $(this).closest(".form-group").remove();
        });
    }

    $(document).ready(function () {
        setupButtons();
    });

} ());