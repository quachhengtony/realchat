const accessToken = "3796899bd37c423bad3a21a25277bce0";
const baseUrl = "https://api.api.ai/api/query?v=2015091001";
const sessionId = "20150910";
const loader = `<span class='loader'><span class='loader__dot'></span><span class='loader__dot'></span><span class='loader__dot'></span></span>`;
const errorMessage =
  "My apologies, I'm not avail at the moment, however, feel free to call our support team directly 0123456789.";
const urlPattern =
  /(\b(https?|ftp):\/\/[-A-Z0-9+&@#\/%?=~_|!:,.;]*[-A-Z0-9+&@#\/%=~_|])/gim;
const $document = document;
const $chatbot = $document.querySelector(".chatbot");
const $chatbotMessageWindow = $document.querySelector(
  ".chatbot__message-window"
);
const $chatbotHeader = $document.querySelector(".chatbot__header");
const $chatbotMessages = $document.querySelector(".chatbot__messages");
const $chatbotInput = $document.querySelector(".chatbot__input");
const $chatbotSubmit = $document.querySelector(".chatbot__submit");

const botLoadingDelay = 1000;
const botReplyDelay = 2000;

const defaultUser = "Tony";
const defaultChatbotId = "1c1c7af5-0de3-4016-ab4a-343baa6c7155";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("https://localhost:7090/chathub")
  .configureLogging(signalR.LogLevel.Information)
  .build();

async function start() {
  try {
    // aiMessage(loader, true, botLoadingDelay);
    await connection.start();
    console.log("SignalR Connected.");
  } catch (err) {
    console.log(err);
    setTimeout(start, 5000);
  }
}
connection.on("Chat", (user, messagee) => {
  console.log(user + " said " + messagee);
  if (user == "DMSBot") {
    setResponse({
      fulfillment: {
        messages: [
          {
            speech: messagee,
            type: 0,
          },
        ],
      },
    });
  }

  if (user == "CTX") {
    setResponse({
      fulfillment: {
        messages: [
          {
            speech: messagee,
            type: 1,
            title: "Tài liệu",
            subtitle: messagee,
          },
        ],
      },
    });
  }

  if (user == "CATS") {
    setResponse({
      fulfillment: {
        messages: [
          {
            type: 2,
            title: "Các nhóm hàng:",
            replies: messagee.split("<CATEGORY>"),
          },
        ],
      },
    });
  }

  if (user == "PRODS_IN_CAT") {
    if (messagee == "[]") {
      setResponse({
        fulfillment: {
          messages: [
            {
              type: 2,
              title: "Các sản phẩm thuộc nhóm hàng:",
              replies: [],
            },
          ],
        },
      });
    } else {
      setResponse({
        fulfillment: {
          messages: [
            {
              payloadType: "PRODUCT_ID",
              type: 2,
              title: "Các sản phẩm thuộc nhóm hàng:",
              replies: messagee.split("<PRODUCT>"),
            },
          ],
        },
      });
    }
  }

  if (user == "PRODS") {
    if (messagee == "[]") {
      setResponse({
        fulfillment: {
          messages: [
            {
              type: 2,
              title: "Các sản phẩm thuộc được tìm thấy:",
              replies: [],
            },
          ],
        },
      });
    } else {
      setResponse({
        fulfillment: {
          messages: [
            {
              payloadType: "PRODUCT_ID",
              type: 2,
              title: "Các sản phẩm được tìm thấy:",
              replies: messagee.split("<PRODUCT>"),
            },
          ],
        },
      });
    }
  }

  if (user == "PROD") {
    if (messagee == "[]") {
      setResponse({
        fulfillment: {
          messages: [
            {
              type: 3,
              title: "Không có sản phẩm nào.",
              replies: [],
            },
          ],
        },
      });
    } else {
      setResponse({
        fulfillment: {
          messages: [
            {
              payloadType: "PRODUCT_ID",
              type: 3,
              title: "Thông tin chi tiết của sản phẩm:",
              replies: messagee.split("<PRODUCT>"),
            },
          ],
        },
      });
    }
  }

  if (user == "LINK") {
    setResponse({
      fulfillment: {
        messages: [
          {
            payloadType: "PRODUCT_ID",
            type: 6,
            title: "Chi tiết sản phẩm có thể xem tại đây:",
            replies: messagee.split("<PRODUCT>"),
          },
        ],
      },
    });
  }
});

connection.on("Register", async () => {
  await connection.invoke("Register", defaultUser);
});

connection.onclose(async () => {
  await start();
});

// Start the connection.
start();

document.addEventListener(
  "keypress",
  (event) => {
    if (event.which == 13) validateMessage();
  },
  false
);

$chatbotHeader.addEventListener(
  "click",
  () => {
    toggle($chatbot, "chatbot--closed");
    $chatbotInput.focus();
  },
  false
);

$chatbotSubmit.addEventListener(
  "click",
  () => {
    validateMessage();
  },
  false
);

const toggle = (element, klass) => {
  const classes = element.className.match(/\S+/g) || [],
    index = classes.indexOf(klass);
  index >= 0 ? classes.splice(index, 1) : classes.push(klass);
  element.className = classes.join(" ");
};

const userMessage = (content) => {
  $chatbotMessages.innerHTML += `<li class='is-user animation'>
      <p class='chatbot__message'>
        ${content}
      </p>
      <span class='chatbot__arrow chatbot__arrow--right'></span>
    </li>`;
};

const aiMessage = (content, isLoading = false, delay = 0) => {
  setTimeout(() => {
    removeLoader();
    $chatbotMessages.innerHTML += `<li 
      class='is-ai animation' 
      id='${isLoading ? "is-loading" : ""}'>
        <div class="is-ai__profile-picture">
          <svg class="icon-avatar" viewBox="0 0 32 32">
            <use xlink:href="#avatar" />
          </svg>
        </div>
        <span class='chatbot__arrow chatbot__arrow--left'></span>
        <div class='chatbot__message'>${content}</div>
      </li>`;
    scrollDown();
  }, delay);
};

const removeLoader = () => {
  let loadingElem = document.getElementById("is-loading");
  if (loadingElem) $chatbotMessages.removeChild(loadingElem);
};

const escapeScript = (unsafe) => {
  const safeString = unsafe
    .replace(/</g, " ")
    .replace(/>/g, " ")
    .replace(/&/g, " ")
    .replace(/"/g, " ")
    .replace(/\\/, " ")
    .replace(/\s+/g, " ");
  return safeString.trim();
};

const linkify = (inputText) => {
  return inputText.replace(urlPattern, `<a href='$1' target='_blank'>$1</a>`);
};

const validateMessage = () => {
  const text = $chatbotInput.value;
  const safeText = text ? escapeScript(text) : "";
  if (safeText.length && safeText !== " ") {
    resetInputField();
    userMessage(safeText);
    send(safeText);
  }
  scrollDown();
  return;
};

const multiChoiceAnswer = (text, text2, payloadType) => {
  const decodedText = text.replace(/zzz/g, "'");

  const decodedText2 = text2.replace(/zzz/g, "'");
  userMessage(decodedText2);
  send(decodedText, payloadType);
  scrollDown();
  return;
};

const processResponse = (val) => {
  if (val && val.fulfillment) {
    let output = "";
    let messagesLength = val.fulfillment.messages.length;

    for (let i = 0; i < messagesLength; i++) {
      let message = val.fulfillment.messages[i];
      let type = message.type;

      switch (type) {
        // 0 fulfillment is text
        case 0:
          let parsedText = linkify(message.speech);
          output += `<p>${parsedText}</p>`;
          break;

        // 1 fulfillment is card
        case 1:
          let imageUrl = message.imageUrl;
          let imageTitle = message.title;
          let imageSubtitle = message.subtitle;
          // let button = message.buttons[0];

          // if (! && !imageTitle && !imageSubtitle) break;

          // output += `
          //   <a class='card' href='${button.postback}' target='_blank'>
          //     <img src='${imageUrl}' alt='${imageTitle}' />
          //   <div class='card-content'>
          //     <h4 class='card-title'>${imageTitle}</h4>
          //     <p class='card-title'>${imageSubtitle}</p>
          //     <span class='card-button'>${button.text}</span>
          //   </div>
          //   </a>
          // `;
          output += `
          <a class='card' target='_blank'>
          <div class='card-content'>
            <h4 class='card-title'>${imageTitle}</h4>
            <p class='card-title'>${imageSubtitle}</p>
          </div>
          </a>
        `;
          break;

        // 2 fulfillment is a quick reply with multi-choice buttons
        case 2:
          let title = message.title;
          let replies = message.replies;
          let repliesLength = replies.length;
          output += `<p>${title}</p>`;

          if (repliesLength <= 0) {
            output += `<p>Không có dữ liệu.</p>`;
          } else {
            for (let i = 0; i < repliesLength - 1; i++) {
              let reply = replies[i];
              // console.log("replies " + reply)
              // let encodedText = toString(reply).replace(/'/g, "zzz");
              output += `<button onclick='multiChoiceAnswer("zzz${
                reply.split(";")[0]
              }zzz", "zzz${reply.split(";")[2]}zzz", "${
                message.payloadType
              }")'>${reply.split(";")[2]}</button>`;
              // output += `<a onclick='send("${replies[i]}")'>${replies[i]}</a>`;
            }
          }

          break;

        case 3:
          output += `
          <a class='card' target='_blank'>
          <div class='card-content'>
            <h4 class='card-title'>${message.replies[0].split(";")[1]}</h4>
            <p class='card-title'>Id: ${message.replies[0].split(";")[0]}</p>
            <p class='card-title'>Mô tả: ${message.replies[0].split(";")[2]}</p>
          </div>
          </a>
        `;
          break;

        case 6:
          output += `<p>${message.title}</p><a href="#">${message.replies[0]}</a>`;
          break;
      }
    }

    // removeLoader();
    return output;
  }

  removeLoader();
  return `<p>${errorMessage}</p>`;
};

const setResponse = (val, delay = 0) => {
  // let endTime = Date.now();
  // if (endTime - startTime < 1000)
  // {
  //   delay = 1000;
  // }
  delay = 1500;
  setTimeout(() => {
    // console.log(`response after ${endTime - startTime} ${startTime}`)
    aiMessage(processResponse(val));
  }, delay);
};

const resetInputField = () => {
  $chatbotInput.value = "";
};

const scrollDown = () => {
  const distanceToScroll =
    $chatbotMessageWindow.scrollHeight -
    ($chatbotMessages.lastChild.offsetHeight + 60);
  $chatbotMessageWindow.scrollTop = distanceToScroll;
  return false;
};

const send = (text = "", payloadType = "") => {
  // fetch(`${baseUrl}&query=${text}&lang=en&sessionId=${sessionId}`, {
  //   method: "GET",
  //   dataType: "json",
  //   headers: {
  //     Authorization: "Bearer " + accessToken,
  //     "Content-Type": "application/json; charset=utf-8",
  //   },
  // })
  //   .then((response) => response.json())
  //   .then((res) => {
  //     if (res.status < 200 || res.status >= 300) {
  //       let error = new Error(res.statusText);
  //       throw error;
  //     }
  //     return res;
  //   })
  //   .then((res) => {
  //     setResponse(res.result, botLoadingDelay + botReplyDelay);
  //   })
  //   .catch((error) => {
  //     setResponse(errorMessage, botLoadingDelay + botReplyDelay);
  //     resetInputField();
  //     console.log(error);
  //   });

  if (payloadType == "PRODUCT_ID") {
    connection.invoke(
      "Chat",
      defaultUser,
      `!PRODUCT_ID:${text}`,
      defaultChatbotId
    );
  } else {
    connection.invoke("Chat", defaultUser, text, defaultChatbotId);
  }
  aiMessage(loader, true, botLoadingDelay);
  resetInputField();

  // aiMessage(loader, true, botLoadingDelay);

  // removeLoader();
};
