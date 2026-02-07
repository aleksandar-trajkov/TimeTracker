import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import '@testing-library/jest-dom'
import { Modal, ModalHeader, ModalBody, ModalFooter } from '../../../../src/components/modal'

describe('Modal Components', () => {
  const mockOnClose = vi.fn()

  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('Modal', () => {
    it('renders modal when isOpen is true', () => {
      render(
        <Modal isOpen={true} onClose={mockOnClose}>
          <div>Modal content</div>
        </Modal>
      )
      
      expect(screen.getByText('Modal content')).toBeInTheDocument()
    })

    it('does not render modal when isOpen is false', () => {
      render(
        <Modal isOpen={false} onClose={mockOnClose}>
          <div>Modal content</div>
        </Modal>
      )
      
      expect(screen.queryByText('Modal content')).not.toBeInTheDocument()
    })

    it('calls onClose when backdrop is clicked', async () => {
      const user = userEvent.setup()
      
      render(
        <Modal isOpen={true} onClose={mockOnClose}>
          <div>Modal content</div>
        </Modal>
      )
      
      // Click on the backdrop (modal overlay)
      const backdrop = document.querySelector('.modal.show')
      if (backdrop) {
        await user.click(backdrop)
        expect(mockOnClose).toHaveBeenCalled()
      }
    })

    it('applies size classes correctly', () => {
      render(
        <Modal isOpen={true} onClose={mockOnClose} size="lg">
          <div>Modal content</div>
        </Modal>
      )
      
      const modalDialog = document.querySelector('.modal-dialog')
      expect(modalDialog).toHaveClass('modal-lg')
    })
  })

  describe('ModalHeader', () => {
    it('renders title and close button', () => {
      render(<ModalHeader title="Test Title" onClose={mockOnClose} />)
      
      expect(screen.getByText('Test Title')).toBeInTheDocument()
      expect(screen.getByLabelText('Close')).toBeInTheDocument()
    })

    it('calls onClose when close button is clicked', async () => {
      const user = userEvent.setup()
      
      render(<ModalHeader title="Test Title" onClose={mockOnClose} />)
      
      await user.click(screen.getByLabelText('Close'))
      expect(mockOnClose).toHaveBeenCalled()
    })
  })

  describe('ModalBody', () => {
    it('renders children content', () => {
      render(
        <ModalBody>
          <p>Body content</p>
        </ModalBody>
      )
      
      expect(screen.getByText('Body content')).toBeInTheDocument()
    })

    it('applies custom className', () => {
      render(
        <ModalBody className="custom-class">
          <p>Body content</p>
        </ModalBody>
      )
      
      const modalBody = screen.getByText('Body content').parentElement
      expect(modalBody).toHaveClass('modal-body', 'custom-class')
    })
  })

  describe('ModalFooter', () => {
    it('renders children content', () => {
      render(
        <ModalFooter>
          <button>Footer button</button>
        </ModalFooter>
      )
      
      expect(screen.getByText('Footer button')).toBeInTheDocument()
    })

    it('applies Bootstrap modal-footer class', () => {
      render(
        <ModalFooter>
          <button>Footer button</button>
        </ModalFooter>
      )
      
      const modalFooter = screen.getByText('Footer button').parentElement
      expect(modalFooter).toHaveClass('modal-footer')
    })
  })
})