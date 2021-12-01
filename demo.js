
function doSomethingAsync(id, callback) {
    "use strict";
    let debug = true;

    if (debug) console.log("Doing something on " + id + " with callback " + callback);

    callback.invokeMethodAsync("SomethingDoneAsync", "Done it!");
}

function listenForDemoValueChange(id, callback) {
    "use strict";
    let debug = true;

    if (debug) console.log("Attaching listener to " + id + " with callback " + callback);

    let element = document.getElementById(id);

    element.addEventListener("input", (evt) => {

        if (debug) console.log("input has changed");
        // For example:
        //   callback.invokeMethodAsync("OnKeyPressInternal", element.value.toString());
    });
}

function rotateDemo(id, callback) {
    "use strict";
    let debug = true;

    if (debug) console.log("Rotating " + id + " with callback " + callback);

    $('.demo-rotate').toggleClass('demo-do-rotate');
    callback.invokeMethodAsync("SomethingDoneAsync", "Done it!");
}


