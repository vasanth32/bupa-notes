# Health Insurance Project - Knowledge Transfer Notes

## Business Overview

This is a **Health Insurance Policy Management System** similar to how employees at Hexaware have group health insurance policies. The system manages the complete lifecycle of health insurance policies from package selection to claim processing.

## Core Business Process Flow

Customer selects Insurance Package
        ↓
Premium Calculation (based on members, options)
        ↓
Payment
        ↓
Policy / Membership Created
        ↓
Digital Insurance Card Generated
        ↓
Customer Uses Healthcare Services
        ↓
Claim Submitted
        ↓
Refund / Settlement Process


### 1. Package Selection
- Customers choose a health insurance package
- Packages can include various options (e.g., including parents, in-laws, etc.)

### 2. Premium Calculation
- Premium is calculated based on:
  - **Number of persons** covered
  - **Options selected** (parents, in-laws, etc.)
  - **No-claim bonus/discounts** (similar to vehicle insurance)
    - If no accidents/claims in current year → discount/bonus for next year

### 3. Payment
- Customers pay the calculated premium
- Payment processing is handled through the system

### 4. Digital Card Generation
- After payment, customers receive a **digital card**
- Similar to Hexaware's group policy ID card with barcode
- Used for accessing healthcare services

### 5. Membership & Sales Process
- To get membership, customers need to meet with a sales person
- Sales process is involved in the membership acquisition

### 6. Claim Processing
- Customers visit healthcare **providers** (hospitals, clinics, etc.)
- Customers choose and use healthcare services
- Submit claims for reimbursement
- Get refunds after claim approval

## Customer Types

### 1. Regular Customers
- Standard health insurance customers

### 2. Overseas Students (OSHC)
- **OSHC** = Overseas Student Health Cover
- Special category for international students coming to Australia
- Australia is famous for higher education, attracting students from across the world
- This is a special term/feature in the application

## System Architecture - Domain APIs

The application is organized into **6 Core Domain APIs**, each with their own endpoints:

```
┌─────────────────────────────────────────────────────────────┐
│                    Health Insurance System                   │
│                     6 Core Domain APIs                       │
└─────────────────────────────────────────────────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        │                     │                     │
        ▼                     ▼                     ▼
┌───────────────┐    ┌───────────────┐    ┌───────────────┐
│   Customer    │    │ Package/Policy │    │   Premium     │
│  Domain API   │───▶│  Domain API    │───▶│  Domain API   │
│               │    │               │    │               │
│ - Customer    │    │ - Packages    │    │ - Calculate   │
│   Info        │    │ - Options     │    │   Premium     │
│ - Types       │    │ - Config      │    │ - Discounts   │
│   (OSHC)      │    │               │    │               │
└───────┬───────┘    └───────┬───────┘    └───────┬───────┘
        │                    │                    │
        │                    │                    │
        ▼                    ▼                    ▼
┌───────────────┐    ┌───────────────┐    ┌───────────────┐
│     Sales     │    │    Payment    │    │     Claim     │
│  Domain API   │───▶│  Domain API   │───▶│  Domain API   │
│               │    │               │    │               │
│ - Sales       │    │ - Process     │    │ - Submit      │
│   Process     │    │   Payment     │    │   Claims      │
│ - Membership  │    │ - Transactions │    │ - Providers   │
│               │    │               │    │ - Refunds     │
└───────────────┘    └───────────────┘    └───────────────┘
```

### 1. **Customer Domain API**
- Manages customer information
- Handles different customer types (regular, OSHC students, etc.)

### 2. **Package/Policy Domain API**
- Manages insurance packages
- Handles package options and configurations

### 3. **Premium Domain API**
- Calculates premiums based on:
  - Number of persons
  - Selected options
  - No-claim bonuses/discounts

### 4. **Sales Domain API**
- Manages the sales process
- Handles membership acquisition through sales personnel

### 5. **Payment Domain API**
- Processes premium payments
- Handles payment transactions

### 6. **Claim Domain API**
- Manages claim submissions
- Processes claims involving healthcare providers
- Handles refunds and reimbursements

## Key Concepts

### No-Claim Bonus
- Similar to vehicle insurance concept
- If customer has no claims/accidents in current year
- Next year premium gets discount/bonus

### Digital Card
- Digital insurance card with barcode
- Issued after successful payment
- Used to access healthcare services
- Similar to Hexaware employee group policy card

### Healthcare Providers
- Hospitals, clinics, medical service providers
- Customers visit providers and use services
- Claims are submitted for services used

### OSHC (Overseas Student Health Cover)
- Special health insurance category for international students in Australia
- Important term to remember in the application

## Business Flow Summary

```
Customer Selection → Package Selection → Premium Calculation → 
Sales Process → Payment → Digital Card Generation → 
Healthcare Service Usage → Claim Submission → Refund Processing
```

## Important Notes

- **Top-down view** of the business and technical architecture
- Each domain API has its own endpoints
- The system handles the complete insurance lifecycle
- Special focus on OSHC for international students
- No "Maverick concept" - all team members are equal contributors

---

*Notes extracted from KT session with Parasuraman Balasubramanian*


## Core Business Flow Diagram

```
┌─────────────────────────┐
│  Customer Registration  │
└────────────┬────────────┘
             │
             ▼
      ┌──────────────┐
      │ Customer Type│
      └──────┬───────┘
             │
      ┌──────┴───────┐
      │              │
      ▼              ▼
┌──────────┐   ┌──────────┐
│ Regular  │   │  OSHC    │
│ Customer │   │ Student  │
└────┬─────┘   └────┬─────┘
     │              │
     └──────┬───────┘
            │
            ▼
┌───────────────────────────┐
│ Select Insurance Package  │
└────────────┬──────────────┘
             │
             ▼
      ┌──────────────┐
      │ Select Options│
      └──────┬───────┘
             │
      ┌──────┼───────┐
      │      │       │
      ▼      ▼       ▼
┌────────┐ ┌────────┐ ┌──────────────┐
│Include │ │Include │ │Number of     │
│Parents │ │In-Laws │ │Persons       │
└───┬────┘ └───┬────┘ └──────┬───────┘
    │          │             │
    └──────┬───┴─────────────┘
           │
           ▼
┌──────────────────────────┐
│   Premium Calculation    │
└────────────┬─────────────┘
             │
             ▼
      ┌──────────────┐
      │ No-Claim     │
      │ Bonus from   │
      │ Previous Year│
      └──────┬───────┘
             │
      ┌──────┴───────┐
      │              │
    Yes│              │No
      ▼              ▼
┌──────────────┐  ┌──────────────┐
│ Apply        │  │              │
│ Discount/    │  │              │
│ Bonus        │  │              │
└──────┬───────┘  │              │
       │          │              │
       └──────┬───┘              │
              │                  │
              ▼                  │
┌──────────────────────────────┐│
│   Sales Process              ││
│   (Meet Sales Person)        ││
└────────────┬─────────────────┘│
             │                  │
             ▼                  │
┌──────────────────────────────┐│
│   Membership Acquisition     ││
└────────────┬─────────────────┘│
             │                  │
             ▼                  │
┌──────────────────────────────┐│
│   Payment Processing         ││
└────────────┬─────────────────┘│
             │                  │
             ▼                  │
      ┌──────────────┐         │
      │   Payment    │         │
      │  Successful? │         │
      └──────┬───────┘         │
             │                 │
      ┌──────┴───────┐         │
      │              │         │
     No│              │Yes      │
      ▼              ▼         │
┌──────────────┐  ┌──────────────┐
│ Retry        │  │ Generate     │
│ Payment      │  │ Digital Card │
│              │  │ (with Barcode)│
└──────┬───────┘  └──────┬───────┘
       │                 │
       └────────┬────────┘
                │
                ▼
┌──────────────────────────┐
│     Policy Active        │
└────────────┬─────────────┘
             │
             ▼
┌──────────────────────────┐
│ Use Healthcare Services  │
│   (Visit Providers)      │
└────────────┬─────────────┘
             │
             ▼
┌──────────────────────────┐
│     Submit Claim         │
└────────────┬─────────────┘
             │
             ▼
┌──────────────────────────┐
│    Claim Processing      │
└────────────┬─────────────┘
             │
             ▼
      ┌──────────────┐
      │   Claim      │
      │  Approved?   │
      └──────┬───────┘
             │
      ┌──────┴───────┐
      │              │
    Yes│              │No
      ▼              ▼
┌──────────────┐  ┌──────────────┐
│ Process      │  │   Claim      │
│ Refund       │  │   Rejected   │
└──────┬───────┘  └──────┬───────┘
       │                 │
       └────────┬────────┘
                │
                ▼
┌──────────────────────────┐
│   Next Year Renewal      │
└────────────┬─────────────┘
             │
             │ (Loop back to No-Claim Bonus check)
             │
             └──────────────────┐
                                │
                                ▼
                        ┌──────────────┐
                        │ No-Claim     │
                        │ Bonus Check  │
                        └──────────────┘
```
