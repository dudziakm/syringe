$(document).ready(function () {
    const rowsToAdd = [
        { $Button: $("#addVerification"), URL: "/Test/AddVerification", Prefix: "Assertions" },
        { $Button: $("#addParsedItem"), URL: "/Test/AddCapturedVariableItem", Prefix: "CapturedVariables" },
        { $Button: $("#addHeaderItem"), URL: "/Test/AddHeaderItem", Prefix: "Headers" },
        { $Button: $("#addVariableItem"), URL: "/TestFile/AddVariableItem", Prefix: "Variables" }
    ];
    let rowHandler = new RowHandler(rowsToAdd);
    rowHandler.setupButtons();

    $("body").on("click", "#removeRow", function (e) {
        e.preventDefault();
        $(this).closest(".form-group").remove();
    });
});