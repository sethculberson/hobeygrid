full_name = "Bobby Fart (C)"
first_name = full_name.split(' ', 1)[0] if ' ' in full_name else full_name
pos = full_name.split('(')[-1].strip(')') if '(' in full_name else ""
full_name = full_name.split('(')[0].strip()                     
first_name = full_name.split(' ', 1)[0] if ' ' in full_name else full_name
last_name = full_name.split(' ', 1)[1] if ' ' in full_name else ""
full_name = f"{first_name} {last_name}".strip()


print(full_name)
print(pos)