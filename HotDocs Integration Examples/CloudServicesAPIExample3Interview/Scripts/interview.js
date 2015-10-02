
$(function () {
    debugger;
    //$.ajax({
    //    type: "POST",
    //    url: "/Home/GetInterviewResponse",
    //    success: function(interview) {
    //        console.log("Done:" + interview);
    //        alert("here");
    //        $("#interview").append(interview);
    //    },
    //});
    $.get("/Home/",
        null, GetExchangeRatesCompleted, "text");
});
function GetExchangeRatesCompleted(result) {
    
    // TODO: Hide *loading* animation.
    // TODO: Display result on page.
}
