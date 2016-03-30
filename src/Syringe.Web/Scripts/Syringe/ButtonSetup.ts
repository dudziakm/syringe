$(document).ready(function () {
    const rowsToAdd = [
        { $Button: $("#addVerification"), URL: "/TestCase/AddVerification", Prefix: "Assertions" },
        { $Button: $("#addParsedItem"), URL: "/TestCase/AddCapturedVariableItem", Prefix: "CapturedVariables" },
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