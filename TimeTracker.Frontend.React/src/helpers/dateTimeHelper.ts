import { DateTime } from "luxon";

const dateFormat = 'yyyy-MM-dd';
const timeFormat = 'HH:mm';

const formatTime = (date: Date): string => {
    return DateTime.fromJSDate(date).setZone('UTC').toFormat(timeFormat);
};
const formatDate = (date: Date): string => {
    return DateTime.fromJSDate(date).setZone('UTC').toFormat(dateFormat);
}
const getToday = (): Date => {
    return DateTime.now().setZone('UTC').startOf('day').toJSDate();
}
const getTomorrow = (): Date => {
    return DateTime.now().setZone('UTC').plus({ days: 1 }).startOf('day').toJSDate();
}
const getStartOfDay = (date: Date): Date => {
    return DateTime.fromJSDate(date).setZone('UTC').startOf('day').toJSDate();
}
const getStartOfWeek = (date: Date): Date => {
    return DateTime.fromJSDate(date).setZone('UTC').startOf('week').toJSDate();
}
const getEndOfWeek = (date: Date): Date => {
    return DateTime.fromJSDate(date).setZone('UTC').endOf('week').toJSDate();
}
const addDays = (date: Date, days: number): Date => {
    return DateTime.fromJSDate(date).setZone('UTC').plus({ days }).toJSDate();
}
export { dateFormat, timeFormat, formatTime, formatDate, getToday, getTomorrow, getStartOfDay, getStartOfWeek, getEndOfWeek, addDays };