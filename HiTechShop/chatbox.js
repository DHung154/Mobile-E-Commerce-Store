$(document).ready(function () {
    $("#chatBody").append(`<div class="bot">Xin chào! Tôi là trợ lý AI 😊</div>`);
});

function toggleChat() {
    document.getElementById("chatbox").classList.toggle("hidden");
}

document.getElementById("chatToggle").onclick = toggleChat;

function sendMessage() {
    let msg = $("#userMessage").val().trim();
    if (!msg) return;

    $("#chatBody").append(`<div class="user">${msg}</div>`);
    $("#userMessage").val("");

    $("#chatBody").scrollTop($("#chatBody")[0].scrollHeight);

    $.post("/Chat/SendMessage", { message: msg }, function (reply) {

        $("#chatBody").append(`<div class="bot">${reply}</div>`);
        $("#chatBody").scrollTop($("#chatBody")[0].scrollHeight);

    }).fail(function () {
        $("#chatBody").append(`<div class="bot">Lỗi kết nối đến server!</div>`);
    });
}

$("#userMessage").keyup(function (event) {
    if (event.key === "Enter") {
        sendMessage();
    }
});