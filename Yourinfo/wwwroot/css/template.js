let cards = document.querySelectorAll(".card");
let clickMeButtons = document.querySelectorAll("button");
let background = document.querySelector("body");
var isElementClciked = false;

function clickHandler(){ // declare a function that updates the state
  isElementClicked = true;
}
//buttons
let button1 = clickMeButtons[0];
let button2 = clickMeButtons[1];
let button3 = clickMeButtons[2];

// Background Changing Button
// Change the backgrond to a different color, image, or gif. 
button1.addEventListener("click", () => {
  cubePattern();
  //Put in a fucntion here.
  // What outcome will your button have? 
  
});


// Motion buttons
button2.addEventListener("click", () => {
  
  destroyInit();
  //Put in a fucntion here.
  // What outcome will your button have? 
  
});

// Self-Destruct Button
button3.addEventListener("click", () => {
  
  fireworks();
  //Put in a fucntion here.
  // What outcome will your button have? 
  
});









/*******************************Function Defenitions**********************************/
// CHANGE BACKGROUND COLOR ONLY
function changeBackgroundColor(color) {
  background.style.background = color;
}

// CHANGE BACKGROUND TO FIREWORKS GIF 
function fireworks() {
  background.style.background = "url(https://media.giphy.com/media/hqIaXesRGpP44/giphy.gif)"; 
  background.style.backgroundSize = "fill";
}

// CHANGE BACKGROUND TO CUBE PATTERN GIF 
function cubePattern() {
 background.style.background = "url(https://media1.giphy.com/media/3o85gd3noLuSkE4Lkc/giphy.gif)"; 
  background.style.backgroundSize = "cover";
}

// CHANGE BACKGROUND IMAGE OF CHOICE
function changeBackgroundImage(imageURL) {
  let image = "url(" + imageURL + ")" 
  background.style.backgroundImage = image;
}

// Used to refresh the page 
function refreshPage(){
  window.location.href = window.location.href;
}

// Self Destruct 
function destroyInit() {
   background.style.backgroundImage = "url(https://media.giphy.com/media/26FPKsfr1V9Yw4eru/giphy.gif)";
   background.style.backgroundSize = "cover";
  
  //Set Card 2's background to become a siren.
  cards[1].style.background = "url(https://media.giphy.com/media/o4lCfVK06ah2M/giphy.gif)";
  cards[1].style.backgroundSize = "cover";
  cards[1].style.backgroundRepeat = "no-repeat";
  cards[1].style.backgroundPosition = "center";
  
  //Set Card 1 and 3 as the yes and no buttons
  cards[0].textContent = "YES";
  cards[0].style.background = "black";
  cards[2].textContent = "NO";
  cards[2].style.background = "black";
  
  //Creates warning message
  var warning = document.createElement("P");                      
  var warningText = document.createTextNode("Explosion sequence iniciated: do you want to erase this page?");      
  warning.appendChild(warningText);
  
  document.getElementById("warningText").appendChild(warning);
  
  destroyChoice();
  
}

// Handles the clickevents for Card 1 and Card 3 
function destroyChoice() {
  cards[0].addEventListener("click", () => {
    document.body.innerHTML = "";
    background.style.background = "white";
  });
  
  cards[2].addEventListener("click", () => {
    refreshPage();
  });
}