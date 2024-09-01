"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").configureLogging(signalR.LogLevel.None).build();

// Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    if (user.id == currentUser) {
        li.classList.add("out");
    } else {
        li.classList.add("in");
    }

    var chatImg = document.createElement("div");
    chatImg.classList.add("chat-img");
    var img = document.createElement("img");
    img.src = "/imageUser/" + user.image;
    chatImg.appendChild(img);

    var chatBody = document.createElement("div");
    chatBody.classList.add("chat-body");
    var chatMessage = document.createElement("div");
    chatMessage.classList.add("chat-message");
    var senderName = document.createElement("h5");
    senderName.textContent = `${user.firstName} ${user.lastName}`;
    var messageContent = document.createElement("p");
    messageContent.textContent = message;
    chatMessage.appendChild(senderName);
    chatMessage.appendChild(messageContent);
    chatBody.appendChild(chatMessage);

    li.appendChild(chatImg);
    li.appendChild(chatBody);

    document.getElementById("messagesList").appendChild(li);

    document.getElementById("messageInput").value = "";
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    connection.invoke("JoinGroup", articleId).catch(function (err) {
        return console.error(err);
    });
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", articleId, message).catch(function (err) {
        return console.error(err);
    });
    event.preventDefault();
});

window.onunload = function () {
    connection.invoke("LeaveGroup", articleId).catch(function (err) {
        return console.error(err);
    });
}