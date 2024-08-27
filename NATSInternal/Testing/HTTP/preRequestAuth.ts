interface TestCase<TInput, TOutput> {
    input: TInput;
    output: TOutput;
}

function test<TInput, TOutput>(
        testCases: TestCase<TInput, TOutput>[],
        testMethod: (input: TInput) => TOutput): void {
    let succeededCases = 0;
    const failedCaseResults: string[] = [];
    testCases.forEach(testCase => {
        const testOutput = testMethod(testCase.input);
        if (testOutput !== testCase.output) {
            let result = `FAILED: ${testCase.input} => ${testCase.output}`;
            result += ` (${testOutput})`
            failedCaseResults.push(result);
        } else {
            succeededCases += 1;
        }
    });
    
    console.log(`Succeeded ${succeededCases}/${testCases.length} case(s)`);
    if (failedCaseResults.length) {
        failedCaseResults.forEach(result => console.log(result));
    }
}

test<string, string>([
    { input: "2024-01-01T00:00:00", output: "Just now" },
    { input: "2023-01-01T00:00:00", output: "1 year(s) ago" },
    { input: "2023-05-02T12:53:12", output: "8 month(s) ago" },
    { input: "2023-12-01T00:00:00", output: "1 month(s) ago" },
    { input: "2023-12-01T00:00:01", output: "30 day(s) ago" },
    { input: "2023-12-27T21:12:01", output: "4 day(s) ago" },
    { input: "2023-12-31T00:00:00", output: "1 day(s) ago" },
    { input: "2023-12-31T13:30:00", output: "10 hour(s) ago" },
    { input: "2023-12-31T23:00:00", output: "1 hour(s) ago" },
    { input: "2023-12-31T23:00:01", output: "59 minute(s) ago" },
    { input: "2023-12-31T23:59:00", output: "1 minute(s) ago" },
    { input: "2023-12-31T23:59:01", output: "Just now" },
    { input: "2024-01-01T00:01:00", output: "1 minute(s) remaining" },
    { input: "2024-01-01T00:59:00", output: "59 minute(s) remaining" },
], getDeltaTextRelativeToNow);

/**
 * Get the text describing the difference (delta) between the current datetime
 * and the datetime represented by the given string.
 * @param targetString An ISO string representing the `Date` to compare.
 * @returns The text describing the difference.
 */
function getDeltaTextRelativeToNow(targetString: string) {
    const targetDate = new Date(targetString + "+09:00");
    const currentDate = new Date("2024-01-01T00:00:00+09:00");
    const targetTime = targetDate.getTime();
    const currentTime = currentDate.getTime();
    const seconds = (currentTime - targetTime) / 1000;
    const absSeconds = Math.abs(seconds);

    let prefix: string;
    let suffix: string;
    if (seconds > 0) {
        prefix = "";
        suffix = " ago";
    } else {
        prefix = ""
        suffix = " remaining";
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
    let pastDate: Date;
    let futureDate: Date;
    if (targetTime < currentTime) {
        [pastDate, futureDate] = [targetDate, currentDate];
    } else {
        [pastDate, futureDate] = [currentDate, targetDate];
    }

    const oneMonthEarlierOfFutureDate = getOneMonthEarlierDate(futureDate);
    if (pastDate.getTime() > oneMonthEarlierOfFutureDate.getTime()) {
        return prefix + `${Math.floor(days)} day(s)` + suffix;
    }

    const oneYearEarlierOfFutureDate = getOneYearEarlierDate(futureDate);
    if (pastDate.getTime() > oneYearEarlierOfFutureDate.getTime()) {
        const months = getMonthsDifference(pastDate, futureDate);
        return prefix + `${months} month(s)` + suffix;
    }

    const years = getYearsDifference(pastDate, futureDate);
    return prefix + `${years} year(s)` + suffix;
}

/**
 * Get the months difference between the 2 given `Date` objects. The value will be
 * rounded down if it contains decimal part.
 * @param pastDate A `Date` object.
 * @param futureDate A `Date` object which represents later date and time than the
 * `startingDate` parameter.
 * @returns The number of months difference between the given `Date` objects.
 */
function getMonthsDifference(pastDate: Date, futureDate: Date): number {
    const pastTotalMonths = pastDate.getFullYear() * 12 + pastDate.getMonth();
    const futureTotalMonths = futureDate.getFullYear() * 12 + futureDate.getMonth();
    const monthDifference = Math.abs(pastTotalMonths - futureTotalMonths);

    // Add months difference to past date to check if the difference is full.
    // Example: The difference between 2023-12-31 and 2024-01-01 is 1 month, but
    // since the difference in days is only 1 day, it's not a full month difference,
    // so it should be adjusted to 0 month difference (rounded down).
    const daysInPastMonth = getDaysInMonth(
        pastDate.getFullYear(),
        pastDate.getMonth() + 1);
    const simulatingDate = futureDate
    simulatingDate.setDate(Math.min(pastDate.getDate(), daysInPastMonth));
    if (simulatingDate.getTime() > futureDate.getTime()) {
        return monthDifference - 1;
    }

    return monthDifference;
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
    const yearsDifference = futureYear - pastYear;

    // Add years difference to past date to check if the difference is full.
    // Example: The difference between 2023-12 and 2024-01 is 1 year, but since
    // the difference in months is only 1 month, it's not a full year difference,
    // so it should be adjusted to 0 year difference (rounded down).
    const simulatingDate = new Date(pastDate.toISOString());
    simulatingDate.setFullYear(futureDate.getFullYear());
    if (simulatingDate.getTime() > futureDate.getTime()) {
        return yearsDifference - 1;
    }

    return yearsDifference;
}

/**
 * Get the number of total days in the specified month and year.
 * @param year The year to which the month to be checked belongs.
 * @param month The month to be checked.
 * @returns The number of total days in the month.
 */
function getDaysInMonth(year: number, month: number): number {
    const date = new Date(year, month, 0);
    return date.getDate();
}

/**
 * Get the `Date` object representing the date which is exactly 1 year different
 * from the given date. The result date will have the same month, same date
 * (adjusted to the last date of month if exceeds, same time).
 * @param date A `Date` object which the result is based on.
 * @returns A `Date` object representing the date with the exactly 1 year
 * different from the given date.
 */
function getOneYearEarlierDate(date: Date): Date {
    const previousYear = date.getFullYear() - 1;
    const thisMonth = date.getMonth() + 1;
    const daysInPreviousYearThisMonth = getDaysInMonth(previousYear, thisMonth);
    return new Date(
        previousYear,
        thisMonth,
        Math.min(daysInPreviousYearThisMonth, date.getDate() + 1));
}

/**
 * Get a `Date` object representing the moment which is exactly 1 month earlier
 * relative to the given date. 
 * @param date A `Date` object to be based on.
 * @returns A `Date` object representing the moment which is exactly 1 month
 * earlier.
 */
function getOneMonthEarlierDate(date: Date): Date {
    const previousMonth = (date.getMonth() + 1) - 1;
    let previousMonthYear = date.getFullYear();
    if (previousMonth === 12) {
        previousMonthYear -= 1;
    }
    const daysInPreviousMonth = getDaysInMonth(previousMonthYear, previousMonth);
    const oneMonthEarlierDate = new Date(date.toISOString());
    oneMonthEarlierDate.setDate(oneMonthEarlierDate.getDate() - daysInPreviousMonth);
    return oneMonthEarlierDate;
}