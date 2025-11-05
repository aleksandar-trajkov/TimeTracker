import React from 'react';
import type { TimeEntry } from '../../apiCalls/timeEntries';

interface ScheduleListProps {
    timeEntries: TimeEntry[];
    isLoading: boolean;
    isError: boolean;
    error?: Error | null;
}

const ScheduleList: React.FC<ScheduleListProps> = ({ 
    timeEntries, 
    isLoading, 
    isError, 
    error 
}) => {
    const formatTime = (dateTimeString: string): string => {
        return new Date(dateTimeString).toLocaleTimeString('en-US', {
            hour: '2-digit',
            minute: '2-digit'
        });
    };
    const formatDate = (dateTimeString: string): string => {
        return new Date(dateTimeString).toLocaleDateString('en-US', {
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        });
    }

    if (isLoading) {
        return (
            <div className="d-flex justify-content-center">
                <div className="spinner-border" role="status">
                    <span className="visually-hidden">Loading...</span>
                </div>
            </div>
        );
    }

    if (isError) {
        return (
            <div className="alert alert-danger" role="alert">
                <h4 className="alert-heading">Error loading time entries</h4>
                <p>{error?.message || 'Failed to load time entries. Please try again.'}</p>
            </div>
        );
    }

    if (timeEntries.length === 0) {
        return (
            <div className="text-center py-5">
                <h3 className="text-muted">No time entries found</h3>
                <p className="text-muted">Start tracking your time to see entries here.</p>
            </div>
        );
    }

    return (
        <div className="table-responsive">
            <table className="table table-striped table-hover">
                <thead className="table-dark">
                    <tr>
                        <th colSpan={3}>{formatDate(timeEntries[0].from)}</th>
                    </tr>
                    <tr>
                        <th>Start Time</th>
                        <th>End Time</th>
                        <th>Category</th>
                    </tr>
                </thead>
                <tbody>
                    {timeEntries.map((entry) => (
                        <tr key={entry.id}>
                            <td>{formatTime(entry.from)}</td>
                            <td>{entry.to ? formatTime(entry.to) : 'Ongoing'}</td>
                            <td>
                                {entry.category && (
                                    <span className="badge bg-primary">
                                        {entry.category.name}
                                    </span>
                                )}
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default ScheduleList;