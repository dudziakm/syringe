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

        $("body").on("click","#removeRow", function (e) {
            e.preventDefault();
            $(this).parent().remove();
        });
    }

    $(document).ready(function () {
        setupButtons();
    });

}());