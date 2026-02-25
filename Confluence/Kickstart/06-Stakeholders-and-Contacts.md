## Stakeholders & contacts (upstream ownership)

Source: Confluence PDF “Project Stakeholders” exported on 2025-02-26.

### How to use this page

When you change an endpoint, contract, or behaviour, it impacts upstream applications. This page helps you quickly identify **who to loop in** for design reviews, testing, and release coordination.

> Treat this as a snapshot. Ownership changes; validate with the current Teams channel / official stakeholder list.

---

## CBO (Customer Business Operations) upstreams

### Apollo CRM

- **Business owners**: Nina Owczarek / Paulo Furtado
- **Technical owners**: Albert Gouzali / Andre Margono
- **Delivery team**: Badrinath Behera / Alex Crossley (Projects)

### Cyclops

- **Business owners**: Nina Owczarek / Paulo Furtado
- **Technical owner**: Anbarasan Rajangam
- **Delivery owner**: Girish Nalawadi

### Façade & relay services (DMZ)

- **Business owners**: Nina Owczarek / Paulo Furtado
- **Technical owner**: Anbarasan Rajangam
- **Delivery owner**: Girish Nalawadi

### Hugo

- **Business owners**: Nina Owczarek / Paulo Furtado
- **Technical owner**: Anbarasan Rajangam
- **Delivery owner**: Girish Nalawadi

### Hugo batch jobs

- **Business owners**: Nina Owczarek / Paulo Furtado
- **Technical owner**: Anbarasan Rajangam
- **Delivery owner**: Girish Nalawadi

---

## Non‑CBO upstreams (selected list from PDF)

### Digital / channels

- **Titanium**: Business owners Nathan Horgan / Tony Ouk; technical owners Goran Fak / Gautam Bhandarkar; delivery owners include Shamika Seethi
- **Digital cards**: Business owners Terrence He / Hamish McMichael; technical owners Jerome Marson / Andy Prince; delivery owners Andy Chang
- **MyBupa (web)**: Business owners Terrence He / Hamish McMichael; technical owners Gabriel Lopez / Andy Prince; delivery owners Andy Chang
- **MyBupa (mobile)**: Business owners Terrence He / Hamish McMichael; technical owners Jerome Marson / Andy Prince; delivery owners Andy Chang

### Telephony / IVR

- **IVR wrapper / telephony**: Rohit Suvarna; Kashish Goyal; Carla MacDonald

### Corporate / other domains

- **Corporate CPC**: Antony Sideropoulos; Paul Milcev
- **Optical**: Adrian Kemp; Reza Ansari (HS Digital team)
- **Dental**: Guneet Sawhney; Reza Ansari (HS Digital team)
- **Connexion(s) CPC**: Brendon Johnson; Hemalatha Paramasivam; Antony Sideropoulos; Paul Milcev

### Common platform / integration

- **Access Verifications (AVA)**: (Common Platform) Jess Keefe; Jonathan Sher; plus David McHugh / Andy Prince referenced in PDF
- **BizTalk (secondary)**: Giovanni Trono; Ragupathy Balarama Naidu (Middleware Services Team)
- **BOSS (sync only, not upstream)**: Luke Crozier; Prasanth Menon; Sandeepkumar Ballae; Sankar Rao
- **HCA claim event**: Terrence He / Hamish McMichael; Andy Prince; David McHugh

### Connected care (BLUA and related)

The PDF lists multiple Connected Care streams under “Blua Crew” (BLUA app / virtual consultation / chemist delivery / awards, etc.) with owners such as:

- Shannon Orb(o)ns / Jesse Liddelow
- Cameron Biggelaar
- Neda Jamshidi
- Daniel Ng / Daniel Proud
- (and others, depending on the stream)

### Product / provider / telehealth

- **HI product**: Rachel Mathews / Glenn Sheffield; Antony Sideropoulos; Paul Milcev
- **Provider CRM (PMP)**: (ownership listed; validate current owners)
- **Bupa telehealth**
- **HCM**

---

## Practical coordination checklist (useful during delivery)

- **Design change**: confirm impacted upstream(s) and owners before implementation.
- **Testing**: ensure upstream teams are engaged early for SIT/UAT test planning.
- **Release**: confirm cutover plan, feature toggles, backward compatibility, and comms.

