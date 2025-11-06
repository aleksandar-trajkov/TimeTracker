import { format, startOfDay, startOfWeek, endOfWeek, addDays as dateFnsAddDays } from 'date-fns';

const dateFormat = 'yyyy-MM-dd';
const timeFormat = 'HH:mm';

const formatTime = (date: Date): string => {
    return format(date, timeFormat);
};
const formatDate = (date: Date): string => {
    return format(date, dateFormat);
}
const getToday = (): Date => {
    return new Date();
}
const getTomorrow = (): Date => {
    const tomorrow = new Date();
    return dateFnsAddDays(tomorrow, 1);
}
const getStartOfDay = (date: Date): Date => {
    return startOfDay(date);
}
const getStartOfWeek = (date: Date): Date => {
    return startOfWeek(date, { weekStartsOn: 1 });
}
const getEndOfWeek = (date: Date): Date => {
    return endOfWeek(date, { weekStartsOn: 1 });
}
const addDays = (date: Date, days: number): Date => {
    return dateFnsAddDays(date, days);
}
export { dateFormat, timeFormat, formatTime, formatDate, getToday, getTomorrow, getStartOfDay, getStartOfWeek, getEndOfWeek, addDays };