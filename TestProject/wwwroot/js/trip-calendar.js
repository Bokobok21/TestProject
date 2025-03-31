/**
 * Trip Calendar - An integrated date-time range picker for trip scheduling
 */
class TripCalendar {
    constructor(options) {
        // Store references to DOM elements
        this.form = document.querySelector("form")
        this.departureInputId = options.departureInputId
        this.returnInputId = options.returnInputId
        this.departureInput = document.getElementById(options.departureInputId)
        this.returnInput = document.getElementById(options.returnInputId)
        this.departureErrorId = options.departureErrorId
        this.returnErrorId = options.returnErrorId
        this.departureTimeError = document.getElementById(options.departureErrorId)
        this.returnTimeError = document.getElementById(options.returnErrorId)
        this.submitButton = this.form.querySelector('button[type="submit"]')

        // Store trip data
        this.userTrips = options.userTrips || []
        this.currentTripId = options.currentTripId || null
        this.isEditMode = !!options.currentTripId

        // Callback function
        this.onTimeChange = options.onTimeChange || (() => { })

        // Initialize
        this.init()
    }

    init() {
        console.log(userTrips)
        // Create container for the calendar
        this.createCalendarContainer()

        // Create visible inputs
        this.createVisibleInputs()

        // Initialize the date picker
        this.initDatePicker()

        // Hide original inputs
        this.hideOriginalInputs()

        // Set initial values if they exist
        this.setInitialValues()
    }

    createCalendarContainer() {
        // Create container for the calendar
        const container = document.createElement("div")
        container.className = "trip-calendar-wrapper"

        // Create header
        const header = document.createElement("div")
        header.className = "trip-calendar-header"
        header.innerHTML = `
      <h4><i class="fas fa-calendar-alt"></i> График на пътуването</h4>
      <p class="trip-calendar-instructions">Изберете дати и часове на заминаване и връщане</p>
    `

        // Create calendar container
        const calendarContainer = document.createElement("div")
        calendarContainer.id = "trip-calendar"
        calendarContainer.className = "trip-calendar"

        // Create error container
        const errorContainer = document.createElement("div")
        errorContainer.className = "trip-calendar-errors"
        errorContainer.innerHTML = `
      <div id="${this.departureErrorId}" class="trip-time-error"></div>
      <div id="${this.returnErrorId}" class="trip-time-error"></div>
    `

        // Append elements
        container.appendChild(header)
        container.appendChild(calendarContainer)
        container.appendChild(errorContainer)

        // Insert after the original inputs
        const parentElement = this.departureInput.parentElement.parentElement
        parentElement.insertBefore(container, this.departureInput.parentElement.nextSibling)

        // Store references
        this.calendarContainer = calendarContainer
        this.calendarWrapper = container
        this.departureTimeError = document.getElementById(this.departureErrorId)
        this.returnTimeError = document.getElementById(this.returnErrorId)
    }

    createVisibleInputs() {
        // Create wrapper
        const inputWrapper = document.createElement("div")
        inputWrapper.className = "trip-datetime-picker-wrapper"

        // Create visible inputs
        const departureVisibleInput = document.createElement("input")
        departureVisibleInput.type = "text"
        departureVisibleInput.id = "departure-visible"
        departureVisibleInput.className = "form-control trip-datetime-input"
        departureVisibleInput.placeholder = "Изберете час на заминаване"
        departureVisibleInput.readOnly = true

        const returnVisibleInput = document.createElement("input")
        returnVisibleInput.type = "text"
        returnVisibleInput.id = "return-visible"
        returnVisibleInput.className = "form-control trip-datetime-input"
        returnVisibleInput.placeholder = "Изберете час на връщане"
        returnVisibleInput.readOnly = true

        // Create labels
        const departureLabel = document.createElement("label")
        departureLabel.htmlFor = "departure-visible"
        departureLabel.className = "trip-datetime-label"
        departureLabel.innerHTML = '<i class="fas fa-plane-departure"></i> Заминаване'

        const returnLabel = document.createElement("label")
        returnLabel.htmlFor = "return-visible"
        returnLabel.className = "trip-datetime-label"
        returnLabel.innerHTML = '<i class="fas fa-plane-arrival"></i> Връщане'

        // Create input groups
        const departureGroup = document.createElement("div")
        departureGroup.className = "trip-datetime-group"
        departureGroup.appendChild(departureLabel)
        departureGroup.appendChild(departureVisibleInput)

        const returnGroup = document.createElement("div")
        returnGroup.className = "trip-datetime-group"
        returnGroup.appendChild(returnLabel)
        returnGroup.appendChild(returnVisibleInput)

        // Add to wrapper
        inputWrapper.appendChild(departureGroup)
        inputWrapper.appendChild(returnGroup)

        // Insert before calendar container
        this.calendarContainer.parentNode.insertBefore(inputWrapper, this.calendarContainer)

        // Store references
        this.departureVisibleInput = departureVisibleInput
        this.returnVisibleInput = returnVisibleInput
        this.inputWrapper = inputWrapper
    }

    hideOriginalInputs() {
        // Get the parent elements of the original inputs
        const departureParent = this.departureInput.parentElement
        const returnParent = this.returnInput.parentElement

        // Hide the parent elements (usually form-group divs)
        departureParent.style.display = "none"
        returnParent.style.display = "none"
    }

    setInitialValues() {
        if (this.departureInput.value) {
            const departureDate = new Date(this.departureInput.value)
            this.departureVisibleInput.value = this.formatDateTime(departureDate)
        }

        if (this.returnInput.value) {
            const returnDate = new Date(this.returnInput.value)
            this.returnVisibleInput.value = this.formatDateTime(returnDate)
        }
    }

    formatDateTime(date) {
        return date.toLocaleString("bg-BG", {
            year: "numeric",
            month: "2-digit",
            day: "2-digit",
            hour: "2-digit",
            minute: "2-digit",
            hour12: false,
        })
    }

    initDatePicker() {
        // Process user trips to get disabled dates
        const disabledRanges = this.processUserTrips()

        // Create picker options
        const options = {
            element: this.calendarContainer,
            format: "DD.MM.YYYY HH:mm",
            inlineMode: true,
            autoApply: true,
            showTooltip: true,
            singleMode: false,
            numberOfMonths: 2,
            numberOfColumns: 2,
            mobileFriendly: true,
            startDate: this.departureInput.value ? new Date(this.departureInput.value) : null,
            endDate: this.returnInput.value ? new Date(this.returnInput.value) : null,
            minDate: new Date(),
            minDays: 0,
            maxDays: 365,
            showWeekNumbers: false,
            plugins: ["mobilefriendly"],
            dropdowns: {
                minYear: new Date().getFullYear(),
                maxYear: new Date().getFullYear() + 2,
                months: true,
                years: true,
            },
            lockDays: disabledRanges,
            setup: (picker) => {
                // Add time selection
                picker.on("selected", (date1, date2) => {
                    if (!date1) return

                    // Show time selection dialog for departure
                    this.showTimeSelectionDialog(date1.dateInstance, "departure", () => {
                        // After departure time is selected, show time selection for return if available
                        if (date2) {
                            this.showTimeSelectionDialog(date2.dateInstance, "return", () => {
                                // After both times are selected, update inputs and validate
                                this.validateTimes()
                                this.onTimeChange()
                            })
                        } else {
                            // Only departure was selected
                            this.validateTimes()
                            this.onTimeChange()
                        }
                    })
                })
            },
        }

        // Initialize picker
        if (typeof Litepicker !== "undefined") {
            this.picker = new Litepicker(options)

            // Add click handlers for visible inputs
            this.departureVisibleInput.addEventListener("click", () => {
                this.picker.show()
            })

            this.returnVisibleInput.addEventListener("click", () => {
                this.picker.show()
            })

            // Add form submission validation
            this.form.addEventListener("submit", (e) => {
                if (!this.validateTimes()) {
                    e.preventDefault()
                    return false
                }
            })
        } else {
            console.error("Litepicker is not defined. Make sure it is included in the page.")

            // Fallback to original inputs
            const departureParent = this.departureInput.parentElement
            const returnParent = this.returnInput.parentElement

            departureParent.style.display = ""
            returnParent.style.display = ""

            this.calendarWrapper.style.display = "none"
        }
    }

    showTimeSelectionDialog(date, type, callback) {
        // Create modal container
        const modal = document.createElement("div")
        modal.className = "time-selection-modal"

        // Create modal content
        const modalContent = document.createElement("div")
        modalContent.className = "time-selection-content"

        // Create header
        const header = document.createElement("div")
        header.className = "time-selection-header"
        header.innerHTML = `<h4>${type === "departure" ? "Изберете час на заминаване" : "Изберете час на връщане"}</h4>`

        // Create time inputs
        const timeContainer = document.createElement("div")
        timeContainer.className = "time-selection-inputs"

        const hourSelect = document.createElement("select")
        hourSelect.className = "time-select hour-select"
        for (let i = 0; i < 24; i++) {
            const option = document.createElement("option")
            option.value = i
            option.textContent = i.toString().padStart(2, "0")
            hourSelect.appendChild(option)
        }

        const minuteSelect = document.createElement("select")
        minuteSelect.className = "time-select minute-select"
        for (let i = 0; i < 60; i += 5) {
            const option = document.createElement("option")
            option.value = i
            option.textContent = i.toString().padStart(2, "0")
            minuteSelect.appendChild(option)
        }

        const timeLabel = document.createElement("div")
        timeLabel.className = "time-label"
        timeLabel.innerHTML = `<span>Час</span><span>Минути</span>`

        timeContainer.appendChild(timeLabel)
        timeContainer.appendChild(hourSelect)
        timeContainer.appendChild(document.createTextNode(":"))
        timeContainer.appendChild(minuteSelect)

        // Create buttons
        const buttonsContainer = document.createElement("div")
        buttonsContainer.className = "time-selection-buttons"

        const confirmButton = document.createElement("button")
        confirmButton.type = "button"
        confirmButton.className = "btn btn-primary"
        confirmButton.textContent = "Потвърди"

        const cancelButton = document.createElement("button")
        cancelButton.type = "button"
        cancelButton.className = "btn btn-secondary"
        cancelButton.textContent = "Отказ"

        buttonsContainer.appendChild(confirmButton)
        buttonsContainer.appendChild(cancelButton)

        // Assemble modal
        modalContent.appendChild(header)
        modalContent.appendChild(timeContainer)
        modalContent.appendChild(buttonsContainer)
        modal.appendChild(modalContent)

        // Add to body
        document.body.appendChild(modal)

        // Set current time if date already has time set
        if (date.getHours() !== 0 || date.getMinutes() !== 0) {
            hourSelect.value = date.getHours()
            // Find closest 5-minute interval
            const minutes = Math.round(date.getMinutes() / 5) * 5
            minuteSelect.value = minutes >= 60 ? 55 : minutes
        } else {
            // Default to current time
            const now = new Date()
            hourSelect.value = now.getHours()
            // Find closest 5-minute interval
            const minutes = Math.round(now.getMinutes() / 5) * 5
            minuteSelect.value = minutes >= 60 ? 55 : minutes
        }

        // Handle button clicks
        confirmButton.addEventListener("click", () => {
            // Update date with selected time
            date.setHours(Number.parseInt(hourSelect.value, 10))
            date.setMinutes(Number.parseInt(minuteSelect.value, 10))

            // Update input values
            if (type === "departure") {
                this.departureInput.value = date.toISOString().slice(0, 16)
                this.departureVisibleInput.value = this.formatDateTime(date)
            } else {
                this.returnInput.value = date.toISOString().slice(0, 16)
                this.returnVisibleInput.value = this.formatDateTime(date)
            }

            // Remove modal
            document.body.removeChild(modal)

            // Call callback
            if (callback) callback()
        })

        cancelButton.addEventListener("click", () => {
            // Remove modal
            document.body.removeChild(modal)

            // Reset picker
            this.picker.clearSelection()
        })
    }

    processUserTrips() {
        const disabledRanges = []
        // Add all dates from existing trips to disabled dates
        this.userTrips.forEach((trip) => {
            console.log(trip)
            // Skip current trip in edit mode
            if (this.isEditMode && trip.id === this.currentTripId) return

            const tripStart = new Date(trip.start)
            const tripEnd = new Date(trip.end)
            console.log(tripStart)
            console.log(tripEnd)

            disabledRanges.push({
                start: tripStart,
                end: tripEnd,
            })
        })


        const blockedDates = [];

        disabledRanges.forEach(range => {
            let currentDate = new Date(range.start);

            while (currentDate <= range.end) {
                blockedDates.push(currentDate.toISOString().split('T')[0]); // Format YYYY-MM-DD
                currentDate.setDate(currentDate.getDate() + 1);
            }
        });

        return blockedDates
    }

    validateTimes() {
                    console.log('dsa')
        let isValid = true

        // Clear previous errors
        this.departureTimeError.textContent = ""
        this.departureTimeError.classList.remove("visible")
        this.returnTimeError.textContent = ""
        this.returnTimeError.classList.remove("visible")
        this.departureVisibleInput.classList.remove("invalid")
        this.returnVisibleInput.classList.remove("invalid")

        // Validate departure time
        //Left Date
        if (this.departureInput.value) {
            const departureTime = new Date(this.departureInput.value)
            const now = new Date()

            if (departureTime < now) {
                this.departureVisibleInput.classList.add("invalid")
                this.departureTimeError.textContent = "Часът на заминаване не може да бъде в миналото"
                this.departureTimeError.classList.add("visible")
                isValid = false
            }

            // Check for overlapping trips
            //Right Date
            for (const trip of this.userTrips) {
                // Skip current trip in edit mode
                if (this.isEditMode && trip.id === this.currentTripId) continue

                const tripStart = new Date(trip.start)
                const tripEnd = new Date(trip.end)

                if (departureTime >= tripStart && departureTime <= tripEnd) {
                    this.departureVisibleInput.classList.add("invalid")
                    this.departureTimeError.textContent = `Имате друго пътуване от ${trip.startPosition} до ${trip.destination} между ${this.formatDateTime(tripStart)} и ${this.formatDateTime(tripEnd)}`
                    this.departureTimeError.classList.add("visible")
                    isValid = false
                    break
                }
            }
        } else {
            this.departureVisibleInput.classList.add("invalid")
            this.departureTimeError.textContent = "Моля, изберете час на заминаване"
            this.departureTimeError.classList.add("visible")
            isValid = false
        }

        // Validate return time
        if (this.returnInput.value) {
            if (!this.departureInput.value) {
                this.returnVisibleInput.classList.add("invalid")
                this.returnTimeError.textContent = "Първо изберете час на заминаване"
                this.returnTimeError.classList.add("visible")
                isValid = false
            } else {
                const departureTime = new Date(this.departureInput.value)
                const returnTime = new Date(this.returnInput.value)

                if (returnTime <= departureTime) {
                    this.returnVisibleInput.classList.add("invalid")
                    this.returnTimeError.textContent = "Часът на връщане трябва да бъде след часа на заминаване"
                    this.returnTimeError.classList.add("visible")
                    isValid = false
                }

                // Check for overlapping trips
                for (const trip of this.userTrips) {
                    // Skip current trip in edit mode
                    if (this.isEditMode && trip.id === this.currentTripId) continue

                    const tripStart = new Date(trip.start)
                    const tripEnd = new Date(trip.end)

                    if (returnTime >= tripStart && returnTime <= tripEnd) {
                        this.returnVisibleInput.classList.add("invalid")
                        this.returnTimeError.textContent = `Имате друго пътуване от ${trip.startPosition} до ${trip.destination} между ${this.formatDateTime(tripStart)} и ${this.formatDateTime(tripEnd)}`
                        this.returnTimeError.classList.add("visible")
                        isValid = false
                        break
                    }

                    // Check if the new trip completely encompasses an existing trip
                    if (departureTime <= tripStart && returnTime >= tripEnd) {
                        this.returnVisibleInput.classList.add("invalid")
                        this.returnTimeError.textContent = `Имате друго пътуване от ${trip.startPosition} до ${trip.destination} между ${this.formatDateTime(tripStart)} и ${this.formatDateTime(tripEnd)}`
                        this.returnTimeError.classList.add("visible")
                        isValid = false
                        break
                    }
                }
            }
        } else {
            this.returnVisibleInput.classList.add("invalid")
            this.returnTimeError.textContent = "Моля, изберете час на връщане"
            this.returnTimeError.classList.add("visible")
            isValid = false
        }

        // Disable/enable submit button based on validation
        this.submitButton.disabled = !isValid

        return isValid
    }

    // Method to disable the calendar (for non-upcoming trips)
    disable() {
        this.departureVisibleInput.disabled = true
        this.returnVisibleInput.disabled = true
        this.calendarContainer.style.opacity = "0.5"
        this.calendarContainer.style.pointerEvents = "none"
    }

    // Method to enable the calendar
    enable() {
        this.departureVisibleInput.disabled = false
        this.returnVisibleInput.disabled = false
        this.calendarContainer.style.opacity = "1"
        this.calendarContainer.style.pointerEvents = "auto"
    }
}

document.addEventListener("DOMContentLoaded", function () {
    let dateTimeInputs = document.querySelectorAll("input[type=datetime-local]");
    let unavailableTimes = getUnavailableTimes(); // Fetch the already selected times

    dateTimeInputs.forEach(input => {
        input.addEventListener("change", function () {
            let selectedTime = new Date(this.value);
            if (isTimeUnavailable(selectedTime)) {
                alert("The selected time overlaps with an existing booking. Please choose another time.");
                this.value = ""; // Reset the input field
            }
        });
    });

    function getUnavailableTimes() {
        let unavailable = [];
        document.querySelectorAll(".unavailable-time").forEach(el => {
            unavailable.push(new Date(el.dataset.time));
        });
        return unavailable;
    }

    function isTimeUnavailable(selectedTime) {
        return unavailableTimes.some(time => Math.abs(time - selectedTime) < 3600000); // Assuming 1-hour conflict range
    }

    // Disable unavailable times dynamically in input fields
    dateTimeInputs.forEach(input => {
        input.addEventListener("focus", function () {
            let picker = this;
            unavailableTimes.forEach(time => {
                let option = document.createElement("option");
                option.value = time.toISOString().slice(0, 16);
                option.textContent = option.value;
                option.disabled = true;
                picker.appendChild(option);
            });
        });
    });
});
