import React, { useState, useEffect } from 'react';
import type { TimeEntry, Category } from '../../apiCalls/timeEntries';
import { formatDateTime } from '../../helpers/dateTime';
import { Modal, ModalHeader, ModalBody, ModalFooter } from '../../components/modal';
import { Input, Button } from '../../components/common';

interface TimeEntryPopupProps {
    isOpen: boolean;
    onClose: () => void;
    onSave: (timeEntry: Partial<TimeEntry>) => void;
    timeEntry?: TimeEntry | null;
}

interface FormData {
    description: string;
    startTime: string;
    endTime: string;
    categoryName: string;
}

const TimeEntryPopup: React.FC<TimeEntryPopupProps> = ({ isOpen, onClose, onSave, timeEntry = null }) => {
    const [formData, setFormData] = useState<FormData>({
        description: '',
        startTime: '',
        endTime: '',
        categoryName: '',
    });

    // Update form data when timeEntry prop changes
    useEffect(() => {
        if (timeEntry) {            
            setFormData({
                description: timeEntry.description,
                startTime: formatDateTime(timeEntry.from),
                endTime: timeEntry.to ? formatDateTime(timeEntry.to) : '',
                categoryName: timeEntry.category?.name || '',
            });
        } else {
            // Reset form for new entry
            setFormData({
                description: '',
                startTime: '',
                endTime: '',
                categoryName: '',
            });
        }
    }, [timeEntry, isOpen]);

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = (e?: React.FormEvent) => {
        if (e) {
            e.preventDefault();
        }
        
        // Create category object if category name is provided
        const category: Category | null = formData.categoryName 
            ? { id: timeEntry?.category?.id || 0, name: formData.categoryName }
            : null;

        const timeEntryData: Partial<TimeEntry> = {
            id: timeEntry?.id,
            description: formData.description,
            from: new Date(`${formData.startTime}`),
            to: new Date(`${formData.endTime}`),
            category,
        };

        onSave(timeEntryData);
        onClose();
    };

    if (!isOpen) return null;

    return (
        <Modal isOpen={isOpen} onClose={onClose} size="md">
            <ModalHeader 
                title={timeEntry ? 'Edit Time Entry' : 'Add Time Entry'} 
                onClose={onClose} 
            />
            
            <ModalBody>
                <form onSubmit={handleSubmit}>
                    <Input
                        id="description"
                        name="description"
                        type="text"
                        label="Description"
                        value={formData.description}
                        onChange={(value) => setFormData(prev => ({ ...prev, description: value }))}
                        placeholder="Enter task description"
                        required
                    />

                    <div className="row">                        
                        <div className="col-md-6">
                            <Input
                                id="categoryName"
                                name="categoryName"
                                type="text"
                                label="Category"
                                value={formData.categoryName}
                                onChange={(value) => setFormData(prev => ({ ...prev, categoryName: value }))}
                                placeholder="Category name"
                                containerClassName="mb-3"
                            />
                        </div>
                    </div>

                    <div className="row">
                        <div className="col-md-6">
                            <div className="mb-3">
                                <label htmlFor="startTime" className="form-label">
                                    Start Time <span className="text-danger ms-1">*</span>
                                </label>
                                <input
                                    type="text"
                                    id="startTime"
                                    name="startTime"
                                    className="form-control"
                                    value={formData.startTime}
                                    onChange={handleInputChange}
                                    required
                                />
                            </div>
                        </div>
                        
                        <div className="col-md-6">
                            <div className="mb-3">
                                <label htmlFor="endTime" className="form-label">End Time</label>
                                <input
                                    type="text"
                                    id="endTime"
                                    name="endTime"
                                    className="form-control"
                                    value={formData.endTime}
                                    onChange={handleInputChange}
                                />
                            </div>
                        </div>
                    </div>
                </form>
            </ModalBody>

            <ModalFooter>
                <Button 
                    type="button" 
                    variant="secondary" 
                    onClick={onClose}
                >
                    Cancel
                </Button>
                <Button 
                    type="submit" 
                    variant="primary" 
                    onClick={handleSubmit}
                >
                    {timeEntry ? 'Update' : 'Save'}
                </Button>
            </ModalFooter>
        </Modal>
    );
};

export default TimeEntryPopup;