import { useQuery } from '@tanstack/react-query';
import Cookies from 'js-cookie';
import { executeGet } from '../../helpers/fetch';
import type { TimeEntry, TimeEntriesQueryParams } from './types';

const useTimeEntriesQuery = (params: TimeEntriesQueryParams = {}) => {
    return useQuery({
        queryKey: ['timeEntries', params],
        queryFn: async (): Promise<TimeEntry[]> => {
            const token = Cookies.get('token');
            const searchParams = new URLSearchParams();
            
            // Add query parameters if they exist
            if (params.from) searchParams.append('from', params.from);
            if (params.to) searchParams.append('to', params.to);

            const queryString = searchParams.toString();
            const url = `${import.meta.env.VITE_BASE_URL}/v1/time-entries${queryString ? `?${queryString}` : ''}`;
            
            var response =  await executeGet(url, token) as unknown as TimeEntry[]
            return response;
        },
        staleTime: 1000 * 60 * 5, // Consider data stale after 5 minutes
        gcTime: 1000 * 60 * 10, // Keep in cache for 10 minutes
    });
};

export default useTimeEntriesQuery;