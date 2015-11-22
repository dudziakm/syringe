$(document).ready(function () {
    const rowsToAdd = [
        { $Button: $("#addVerification"), URL: "/TestCase/AddVerification", Prefix: "Verifications" },
        { $Button: $("#addParsedItem"), URL: "/TestCase/AddParseResponseItem", Prefix: "ParseResponses" },
        { $Button: $("#addHeaderItem"), URL: "/TestCase/AddHeaderItem", Prefix: "Headers" },
        { $Button: $("#addVariableItem"), URL: "/TestFile/AddVariableItem", Prefix: "Variables" }
    ];
    let rowHandler = new RowHandler(rowsToAdd);
    rowHandler.setupButtons();

    $("body").on("click", "#removeRow", function (e) {
        e.preventDefault();
        $(this).closest(".form-group").remove();
    });
});