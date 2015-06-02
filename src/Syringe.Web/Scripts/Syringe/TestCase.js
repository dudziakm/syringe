(function () {

    function setupButtons() {
        
        $("#addVerification").click(function (e) {
            e.preventDefault();

            var verificationItem = {
                Description: $("#description").val(),
                Regex: $("#regex").val(),
                VerifyType: $("#verifyType").val()
            };
            $.get("/TestCase/AddVerification", verificationItem, function (data) {
                $("#addVerification").parent().before(data);
            });
        });
    }

    $(document).ready(function () {
        setupButtons();
    });

}());