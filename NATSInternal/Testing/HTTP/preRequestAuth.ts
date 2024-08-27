const dateTimeString = "2023-03-01T13:00:00";

console.log(getDeltaTextRelativeToNow(dateTimeString));

function getDeltaTextRelativeToNow(targetString: string) {
    const targetDate = new Date(targetString + "+09:00");
    const currentDate = new Date("2024-02-29T12:00:00+09:00");
    const targetTime = targetDate.getTime();
    const currentTime = currentDate.getTime();
    const seconds = (currentTime - targetTime) / 1000;
    const absSeconds = Math.abs(seconds);

    let prefix;
    let suffix;
    if (seconds > 0) {
        prefix = "";
        suffix = " ago";
    } else {
        prefix = "next "
        suffix = "";
    }
    
    if (absSeconds < 60) {
        return "Just now";
    }

    const minutes = absSeconds / 60;
    if (minutes < 60) {
        return prefix + `${Math.floor(minutes)} minute(s)` + suffix;
    }

    const hours = minutes / 60;
    if (hours < 24) {
        return prefix + `${Math.floor(hours)} hour(s)` + suffix;
    }

    const days = hours / 24;
    let pastDate;
    let futureDate;
    if (targetTime < currentTime) {
        [pastDate, futureDate] = [targetDate, currentDate];
    } else {
        [pastDate, futureDate] = [currentDate, targetDate];
    }

    const daysInPreviousMonthOfFutureDate = getDaysInMonth(
        futureDate.getFullYear(),
        futureDate.getMonth() - 1);
    const oneMonthPriorToFutureDate = new Date(futureDate.toISOString());
    oneMonthPriorToFutureDate
        .setDate(oneMonthPriorToFutureDate.getDate() - daysInPreviousMonthOfFutureDate);
    if (pastDate.getTime() > oneMonthPriorToFutureDate.getTime()) {
        return prefix + `${Math.floor(days)} day(s)` + suffix;
    }

    const oneYearPriorToFutureDate = getOneYearPreviousDate(futureDate);
    console.log(oneYearPriorToFutureDate.toLocaleString());
    if (pastDate.getTime() > oneYearPriorToFutureDate.getTime()) {
        const months = getMonthsDifference(pastDate, futureDate);
        return prefix + `${months} month(s)` + suffix;
    }

    const years = getYearsDifference(pastDate, futureDate);
    return prefix + `${years} year(s)` + suffix;


}

function getMonthsDifference(pastDate: Date, futureDate: Date): number {
    const pastTotalMonths = pastDate.getFullYear() * 12 + pastDate.getMonth();
    const futureTotalMonths = futureDate.getFullYear()* 12 + futureDate.getMonth();
    const monthDifference = Math.abs(pastTotalMonths - futureTotalMonths);
    if (pastTotalMonths !== futureTotalMonths) {
        return monthDifference;
    }
    
    if (pastDate.getTime() < futureDate.getTime()) {
        return monthDifference - 1;
    }

    return monthDifference + 1;
}

/**
 * Get the years difference between the 2 given `Date` objects. The value will be
 * rounded down if it contains decimal part.
 * @param pastDate A `Date` object.
 * @param futureDate A `Date` object which represents later date and time than the
 * `startingDate` parameter.
 * @returns The number of years difference between the given `Date` objects.
 */
function getYearsDifference(pastDate: Date, futureDate: Date): number {
    const pastYear = pastDate.getFullYear();
    const futureYear = futureDate.getFullYear();
    let yearsDifference = futureYear - pastYear;

    // Add years difference to past date to check if the difference is full.
    // Example: The difference between 2023-12 and 2024-01 is 1 year, but since
    // the difference in months is only 1 month, it's not a full year difference,
    // so it should be adjusted to 0 year difference (rounded down).
    const simulatingDate = new Date(pastDate.toISOString());
    simulatingDate.setFullYear(futureDate.getFullYear());
    if (simulatingDate.getTime() > futureDate.getTime()) {
        yearsDifference -= 1;
    }

    return yearsDifference;
}

function getDaysInMonth(year: number, month: number): number {
    const date = new Date(year, month, 0);
    return date.getDate();
}

function getLocaleISOString(date: Date) {
    const localDate = date;
    localDate.setHours(localDate.getHours() + 9);
    const isoString = localDate.toISOString();
    return isoString.slice(0, isoString.length - 5);
}

function getOneYearPreviousDate(date: Date): Date {
    const previousYear = date.getFullYear() - 1;
    const currentYear = date.getFullYear();
    const previousYearDate = new Date(date.toISOString());
    if (currentYear % 4 === 0 || previousYear % 4 === 0) {
        previousYearDate.setDate(previousYearDate.getDate() - 366);
        return previousYearDate;
    }

    previousYearDate.setDate(previousYearDate.getDate() - 365);
    return previousYearDate;
}