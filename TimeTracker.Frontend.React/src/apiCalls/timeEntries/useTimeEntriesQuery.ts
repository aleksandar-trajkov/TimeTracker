import { useQuery } from '@tanstack/react-query';
import Cookies from 'js-cookie';
import { executeGet } from '../../helpers/fetch';
import type { TimeEntry, TimeEntriesQueryParams } from './types';
import { formatDate } from '../../helpers/dateTimeHelper';

const useTimeEntriesQuery = (params: TimeEntriesQueryParams) => {
    return useQuery({
        queryKey: ['timeEntries', params],
        queryFn: async (): Promise<TimeEntry[]> => {
            const token = Cookies.get('token');
            const searchParams = new URLSearchParams();
            
            // Add query parameters if they exist
            if (params.from) searchParams.append('from', formatDate(params.from));
            if (params.to) searchParams.append('to', formatDate(params.to));

            const queryString = searchParams.toString();
            const url = `${import.meta.env.VITE_BASE_URL}/v1/time-entries${queryString ? `?${queryString}` : ''}`;
            
            return await executeGet(url, token) as unknown as TimeEntry[]
        }
    });
};

export default useTimeEntriesQuery;