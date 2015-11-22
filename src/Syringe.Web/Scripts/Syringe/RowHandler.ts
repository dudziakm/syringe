class RowHandler {

    rowsToAdd: IRowAdder[];
    constructor(i: IRowAdder[]) {
        this.rowsToAdd = i;
    }

    private addRow = (e) => {
        e.preventDefault();

        var testCase = e.data.testCase;

        $.get(testCase.URL, function (html) {
            var panelBody = testCase.$Button.parent().next();
            var formGroup = panelBody.find(".form-group:last-child"), rowNumber = 0;

            if (formGroup.length !== 0) {
                var firstInputName = formGroup.find("input:first").attr("name");

                // get the last index number of the row and increment it by 1
                rowNumber = parseInt(firstInputName.match(/\d/g)) + 1;
            }

            // replace the name value with the correct prefix and row number so it can be posted to the server 
            var newHtml = html.replace(/name="/g, "name=\"" + testCase.Prefix + "[" + rowNumber + "].");
            panelBody.append(newHtml);
        });
    }

    public setupButtons = () => {
        for (let i = 0; i < this.rowsToAdd.length; i++) {
            let iTestCase = this.rowsToAdd[i];
            iTestCase.$Button.on("click", { testCase: iTestCase }, this.addRow);
        }
    }
}


