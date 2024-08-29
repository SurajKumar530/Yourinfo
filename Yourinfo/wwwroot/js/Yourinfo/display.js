// JavaScript to handle color picker value
document.addEventListener('DOMContentLoaded', function () {
    // Set default color (e.g., red)
    var defaultColor = '#ff0000';
    document.getElementById('colorPicker').value = defaultColor;
    document.getElementById('selectedColor').value = defaultColor;
    document.getElementById('textcolorPicker').value = defaultColor;
    document.getElementById('selectedTextColor').value = defaultColor;

    // Event listener for color picker change
    document.getElementById('colorPicker').addEventListener('change', function () {
        var selectedColor = this.value;
        document.getElementById('selectedColor').value = selectedColor;
    });

    // Event listener for color picker change
    document.getElementById('textcolorPicker').addEventListener('change', function () {
        var selectedColor = this.value;
        document.getElementById('selectedTextColor').value = selectedColor;
    });


    document.getElementById('imageInput').addEventListener('change', function (event) {
        var file = event.target.files[0];
        var reader = new FileReader();

        reader.onload = function (e) {
            document.getElementById('uploadedImage').setAttribute('src', e.target.result);
        };

        reader.readAsDataURL(file);
    });

    document.getElementById('countryDropdown').addEventListener('change', function () {
        var selectedOption = this.value;
        document.getElementById('displayCountryValue').textContent = selectedOption;
    });

    document.getElementById('stateDropdown').addEventListener('change', function () {
        var selectedOption = this.value;
        document.getElementById('stateValue').textContent = selectedOption;
    });

    document.getElementById('cityDropdown').addEventListener('change', function () {
        var selectedOption = this.value;
        document.getElementById('CityValue').textContent = selectedOption;
    });

    const accordionItems = document.querySelectorAll('.accordion-item');

    accordionItems.forEach(item => {
        const header = item.querySelector('.accordion-header');
        const content = item.querySelector('.accordion-content');

        header.addEventListener('click', () => {
            content.classList.toggle('active');
            // Adjust max-height based on active class
            if (content.classList.contains('active')) {
                content.style.maxHeight = content.scrollHeight + "px";
            } else {
                content.style.maxHeight = 0;
            }
        });
    });
});


$(document).ready(function () {
    $('#countryDropdown').change(function () {
        
        var countryId = $(this).val();
        if (countryId) {
            $.ajax({
                url: '/Register/GetStates',
                type: 'GET',
                dataType: 'json',
                data: { countrytext: countryId },
                success: function (data) {
                    $('#stateDropdown').empty();
                    $('#stateDropdown').append($('<option>').text('Select a state').attr('value', ''));
                    $.each(data.userViewState.dropdownItems, function (index, state) {
                        $('#stateDropdown').append($('<option>').text(state.value).attr('value', state.text));
                    });
                }
            });
        } else {
            $('#stateDropdown').empty();
            $('#stateDropdown').append($('<option>').text('Select a state').attr('value', ''));
        }
    });


    $('#stateDropdown').change(function () {
        
        var countryId = $(this).val();
        if (countryId) {
            $.ajax({
                url: '/Register/GetCity',
                type: 'GET',
                dataType: 'json',
                data: { Statetext: countryId },
                success: function (data) {
                    $('#cityDropdown').empty();
                    $('#cityDropdown').append($('<option>').text('Select a City').attr('value', ''));
                    $.each(data.userViewCity.dropdownItems, function (index, state) {
                        $('#cityDropdown').append($('<option>').text(state.value).attr('value', state.text));
                    });
                }
            });
        } else {
            $('#cityDropdown').empty();
            $('#cityDropdown').append($('<option>').text('Select a City').attr('value', ''));
        }
    });

});


function updateValue(inputId, displayId) {
    var value = document.getElementById(inputId).value;
    document.getElementById(displayId).textContent = value;
}

function changeSidebarTextColor(select) {
    var color = select.value;
    var sidebarElements = document.querySelectorAll('#sidebarDiv');

    sidebarElements.forEach(function (element) {
        element.style.color = color;
    });
}

function changeSidebarColor(select) {
    var color = select.value;
    var sidebar = document.getElementById('sidebarDiv');

    sidebar.style.backgroundColor = color;

}

function updateSidebar(inputId, displayId) {
    // Get the dropdown element
    var value = document.getElementById(inputId).value;
    document.getElementById(displayId).textContent = value;
}

$(document).ready(function () {
    $('.sidebar').removeClass('mobile-display');
    $('.sidebar').removeClass('tablet-display');
    $('.sidebar').removeClass('window-display');

    $('#mobile-tab').click(function () {
        $('.sidebar').removeClass('tablet-display');
        $('.sidebar').removeClass('window-display');
        //$('.sidebar').css('width', '200px');
        $('.sidebar').addClass('mobile-display');// Default width for window
    });

    $('#tablet-tab').click(function () {
        $('.sidebar').removeClass('mobile-display');
        $('.sidebar').removeClass('window-display');

        // Example JavaScript handling for tab click
        $('.sidebar').addClass('tablet-display');
    });

    $('#window-tab').click(function () {
        $('.sidebar').removeClass('mobile-display');
        $('.sidebar').removeClass('tablet-display');
        $('.sidebar').addClass('window-display');
    });
});